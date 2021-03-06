﻿using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Discord;
using Photon.Realtime;

public enum GameMod
{
    MENU = 0,
    GAMEPLAY = 1
}

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static GameManager Instance;


    #endregion

    [Header("Mode Settings")]
    public GameMod gameMod = GameMod.GAMEPLAY;

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public GameObject racketPrefab;

    public Transform[] playerSpawn;
    public Transform[] PlayerSpawn { get => playerSpawn; }

    [Header("Offline Mode")]
    public bool offlineMode = false;

    [Header("Menu Canvas")]
    public GameObject warningPrefab;
    public Transform warningTransform;

    [Header("Multiplayer")]
    public QPlayer startingPlayer;


    public bool IsBrickFreeToMove { get; private set; }
    public bool IsGameStarted { get; private set; }
    public bool IsGamePaused { get; private set; }
    public bool IsGameEnded { get; private set; }
    public bool HasLost { get; private set; }

    [HideInInspector]
    public int levelIndex;

    private bool allPlayersAreReady;
    private Queue<SequenceTask> sequenceTaskQueue;

    private GameObject tempBall;
    private bool isGameplayScene;
    private GUIMenuPause guiMenuPause;


    bool launchingGame = false;
    PhotonView photonView;

    void Awake()
    {
        Instance = this;
        SetupOfflineMod();
        photonView = GetComponent<PhotonView>();

        sequenceTaskQueue = new Queue<SequenceTask>();      //A supprimer
        allPlayersAreReady = false;
        IsGameStarted = false;
        IsGamePaused = false;
        IsGameEnded = false;

        isGameplayScene = false;

        if (offlineMode)
            HasLost = false;
        else
            HasLost = true;
    }

    void Start()
    {
        InstantiatePlayers();
        SpawnLevel();

        if (gameMod == GameMod.GAMEPLAY)
        {
            if (offlineMode || PhotonNetwork.IsMasterClient)
            {
                AddSequenceTaskToQueue(InstanciateBall, false);
                AddSequenceTaskToQueue(SetupBall);

                AddSequenceTaskToQueue(SetupGameRules);

                AddSequenceTaskToQueue(StartGameplayLoop);

                if (offlineMode)
                    ResumeTaskSequence();
                else
                    StartCoroutine(SetupSequenceWaitCoroutine());
            }
            else
            {
                photonView.RPC("BecomeReady", RpcTarget.MasterClient);
            }

            if(offlineMode)
                DiscordManager.Instance?.SetDiscordPresence(GameSituation.Solo, "Level " + LevelManager.instance.CurrentLevelID);
            else
                DiscordManager.Instance?.SetDiscordPresence(GameSituation.Multi);

            isGameplayScene = true;
        }
        else
        {
            isGameplayScene = false;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        launchingGame = false;
    }

    #region SetupMethod

    #region MainSetup

    private void SetupOfflineMod()
    {
        if (offlineMode)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 60;
        }
    }

    [PunRPC]
    private void InstantiatePlayers()
    {
        if (!offlineMode && gameMod == GameMod.GAMEPLAY)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                QPlayerManager.instance.SetLocalPlayer(PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn[0].position, Quaternion.identity, 0) as GameObject);

                if (gameMod == GameMod.GAMEPLAY)
                {
                    RacketManager.instance.SetLocalRacket(PhotonNetwork.Instantiate("RacketPlayer", Vector3.zero, Quaternion.identity) as GameObject);
                }
            }
            else
            {
                QPlayerManager.instance.SetLocalPlayer(PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn[1].position, Quaternion.identity, 0) as GameObject);

                if (gameMod == GameMod.GAMEPLAY)
                {
                    RacketManager.instance.SetLocalRacket(PhotonNetwork.Instantiate("RacketPlayer", Vector3.zero, Quaternion.identity) as GameObject);
                }
            }
        }
        else
        {
            QPlayerManager.instance.SetLocalPlayer(Instantiate(playerPrefab, playerSpawn[0].transform.position, Quaternion.identity));

            if (gameMod == GameMod.GAMEPLAY)
            {
                RacketManager.instance.SetLocalRacket(Instantiate(racketPrefab, Vector3.zero, Quaternion.identity));
            }
        }
    }

    #endregion

    #region GameplaySetup

    private IEnumerator SetupSequenceWaitCoroutine()
    {
        while (!allPlayersAreReady)
        {
            yield return new WaitForFixedUpdate();
        }

        StartSequence();
    }

    [PunRPC]
    public void BecomeReady()
    {
        allPlayersAreReady = true;
    }

    private void SpawnLevel()
    {
        if (offlineMode)
        {
            SelectionLevel(CampaignLevel.instance.levelScriptSelected);
        }
        else
        {
            if (gameMod == GameMod.GAMEPLAY)
            {
                Debug.Log(MultiLevel.Instance.levelIndex);
                //photonView.RPC("SelectionLevelRPC", RpcTarget.All, MultiLevel.Instance.levelIndex);

                SelectionLevelMulti(MultiLevel.Instance.levelIndex);
            }
        }
    }

    public void InstanciateBall()
    {
        if (offlineMode || PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("GameManager Instanciate ball");
            tempBall = BallManager.instance.InstantiateBall();
        }
    }

    public void SetupBall()
    {
        if (offlineMode)
            tempBall.GetComponent<BallSetup>().SetupBall();
        else if (PhotonNetwork.IsMasterClient)
        {
            tempBall.GetComponent<BallSetup>().SendSetupBallRPC();
        }
    }

    public void SetupGameRules()
    {
        //Debug.Log("SetupGameRules");

        // Lire le level scriptable
        // Abonner endofgame/LoseGame
        // Initialize tout ce qu'il faut

        BallEventManager.instance.OnCollisionWithRacket += StartTheGame;

        if (offlineMode)
        {

            //if(LevelManager.instance.currentLevel.level.levelSpec.switchColorBehaviourForThisLevel == ColorSwitchType.NONE)
            //    BallManager.instance.BallColorBehaviour.Initialize(ColorSwitchType.NONE);
            //else
            BallManager.instance.BallColorBehaviour.Initialize(LevelManager.instance.currentLevel.level.levelSpec.switchColorBehaviourForThisLevel);

            if (LevelManager.instance.currentLevel.level.levelSpec.suddenDeath)
            {
                ScoreManager.Instance.OnComboReset += LoseTheGame;
            }

            ////////   Useful ?
            /*
            else 
            {
                TimeManager.Instance.OnTimerEnd += LoseTheGame;
            }
            */

            if (LevelManager.instance.currentLevel.level.levelSpec.timeAttack)
            {
                //TimeManager.Instance.OnTimerEnd += LevelManager.instance.OnTimerNextLayer;
                TimeManager.Instance.OnTimerEnd += LoseTheGame;

                //LevelManager.instance.onLayerEndEvent += TimeManager.Instance.OnTimeAttackBoost;
            }

            if (LevelManager.instance.currentLevel.level.levelSpec.mandatoryBounce)
            {
                LockWallManager.Instance.Initialize();
            }
        }
        else
        {
            BallManager.instance.BallColorBehaviour.Initialize(ColorSwitchType.RACKETEMPOWERED);

            if (PhotonNetwork.IsMasterClient)
                TimeManager.Instance.OnTimerEnd += LoseTheGame;
        }
    }


    #endregion

    #endregion

    #region NetworkSequencer

    #region Sequencer

    private struct SequenceTask
    {
        public Action action;
        public bool isAutomaticallyResumed;

        public SequenceTask(Action nextAction, bool isAutomatic)
        {
            action = nextAction;
            isAutomaticallyResumed = isAutomatic;
        }
    }

    private void AddSequenceTaskToQueue(Action nextAction, bool isAutomaticallyResumed = true)
    {
        if (nextAction != null)
        {
            sequenceTaskQueue.Enqueue(new SequenceTask(nextAction, isAutomaticallyResumed));
        }
    }

    [PunRPC]
    public void ResumeTaskSequence()
    {
        //Debug.Log("ResumeTaskSequence");
        ExecuteNextTask();
    }

    public void ExecuteNextTask()
    {
        if (sequenceTaskQueue.Count > 0)
        {
            SequenceTask newSequenceTask;
            if (offlineMode)
            {
                newSequenceTask = sequenceTaskQueue.Dequeue();
                newSequenceTask.action();
                ResumeTaskSequence();
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                newSequenceTask = sequenceTaskQueue.Dequeue();
                newSequenceTask.action();                                                               // Is there too much latenty?
                photonView.RPC("DistantExecute", RpcTarget.Others, newSequenceTask.action.Method.Name, newSequenceTask.isAutomaticallyResumed);
            }
        }
    }

    [PunRPC]
    private void DistantExecute(string methodName, bool isAutomaticallyResumed)
    {
        MethodInfo methodInfo = this.GetType().GetMethod(methodName);
        if (methodInfo != null)
        {
            methodInfo.Invoke(this, null);
            //ReadyCheck((Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo));

            if (isAutomaticallyResumed)
            {
                photonView.RPC("ResumeTaskSequence", RpcTarget.MasterClient);
                Debug.Log("Send ResumeTaskSequence RPC");
            }
        }
        else
            Debug.LogError("GameManager.DistantExecute(string) : method " + methodName + " doesn't exist!");
    }
    #endregion

    public void SendResumeRPC()
    {
        //Debug.Log("Send ResumeTaskSequence RPC");
        photonView.RPC("ResumeTaskSequence", RpcTarget.MasterClient);
    }

    public void StartSequence()
    {
        ResumeTaskSequence();
    }

    #endregion

    #region GameplayControlMethods

    #region Gameplay Loop

    public void StartGameplayLoop()
    {
        IsGameEnded = false;
        StartBrickMovement();
        BallFirstSpawn();
    }

    [PunRPC]
    public void StartBrickMovement()
    {
        IsBrickFreeToMove = true;
    }

    public void BallFirstSpawn()
    {
        //Debug.Log("BallFirstSpawn");
        BallManager.instance.BallFirstSpawn(startingPlayer);
    }

    public void StartTheGame(Collision collision)
    {
        if (offlineMode)
        {
            StartTheGameRPC();
        }
        else
        {
            photonView.RPC("StartTheGameRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartTheGameRPC()
    {
        if (!IsGameStarted)
        {
            IsGameStarted = true;

            TimeManager.Instance.StartTimer();

            BallEventManager.instance.OnCollisionWithRacket -= StartTheGame;
        }
    }

    public void LoseTheGame()
    {
        HasLost = true;

        EndOfTheGame((int)QPlayerManager.instance.LocalPlayerID);
    }

    public void EndOfTheGame(int playerID)
    {
        if (offlineMode)
            DelayedEndGame();
        else if (playerID == (int)QPlayerManager.instance.LocalPlayerID)
        {
            HasLost = false;
            photonView.RPC("DelayedEndGame", RpcTarget.All);
        }
    }

    [PunRPC]
    private void DelayedEndGame()
    {
        IsGameEnded = true;
        StartCoroutine(EndTheGamelocaly());
    }

    private IEnumerator EndTheGamelocaly()
    {
        IsGameEnded = true;

        TimeManager.Instance.StopTimer();
        LevelManager.instance.CleanWalls();
        BallManager.instance.DespawnTheBall();

        yield return new WaitForSeconds(1f);

        if (!HasLost && offlineMode && LevelManager.instance.currentLevel.level.levelSpec.timeAttack)
            ScoreManager.Instance.OnTimeAttackBonus();

        PlayerInputManager.instance.SetInputMod(InputMod.MENU);
        LevelManager.instance.playersHUD.EnableScoreScreen();

        //If the player who WINS is ALONE
        if (!HasLost && offlineMode)
        {
            //Debug.Log("Submit DATAS at endOfTheGame SOOOO .... NOW");
            JSON.instance.currentLevelFocused = LevelManager.instance.currentLevel;
            JSON.instance.SubmitDATA(LevelManager.instance.currentLevel, (int)ScoreManager.Instance.score[0], ScoreManager.Instance.playersMaxCombo[0], (int)TimeManager.Instance.CurrentTimer);
        }
    }

    #endregion

    #region Pause

    public void PauseGame()
    {
        if (isGameplayScene && !IsGameEnded)
        {
            if (offlineMode)
            {
                //Pause Timer
                TimeManager.Instance.StopTimer();
                //Pause Ball
                BallManager.instance.PauseBall();
                //Pause Level Bricks
                IsBrickFreeToMove = false;

                IsGamePaused = true;
            }

            //Switch Input Mode & Desable Racket
            PlayerInputManager.instance.SetInputMod(InputMod.MENU);
            //Display Pause UI
            LevelManager.instance.guiMenuPause.GamePaused();
        }
    }

    public void ResumeGame()
    {
        if (isGameplayScene && !IsGameEnded)
        {
            if (offlineMode)
            {
                //Unpause Timer
                TimeManager.Instance.StartTimer();
                //UnPause la balle
                BallManager.instance.ResumeBall();
                //Unpause Level Bricks
                IsBrickFreeToMove = true;

                IsGamePaused = false;
            }

            //Switch Input Mode & Enable Racket
            PlayerInputManager.instance.SetInputMod(InputMod.GAMEPLAY);
            //Undisplay Pause UI
            LevelManager.instance.guiMenuPause.GameResumed();
        }
    }

    #endregion

    #endregion

    #region SceneControlMethods
    public void RestartScene()  //Need Network Case
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ReloaSceneCor());
        }
    }

    IEnumerator ReloaSceneCor()
    {
        //send RPC to other clients to load my scene
        photonView.RPC("LoadMyScene", RpcTarget.Others, SceneManager.GetActiveScene().name);
        yield return null;
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name); //restart the game
        PhotonNetwork.DestroyAll();
    }

    [PunRPC]
    public void LoadMyScene(string sceneName)
    {
        Debug.Log("REcieved RPC " + sceneName);
        PhotonNetwork.LoadLevel(sceneName); //restart the game
    }


    public void SelectionLevelMulti(int selection)
    {
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    public void SelectionLevel(LevelsScriptable selection)
    {
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    public void DisconnectGameToMenu()
    {
        if (gameMod == GameMod.GAMEPLAY)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.DestroyAll();
                SceneManager.LoadScene(0);
            }
        }
    }

    private void Update()
    {
        if (!offlineMode)
        {
            DisconnectGameToMenu();
        }

        ///DEBUG POUR TESTER LA FIN DE NIVEAU
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    EndOfTheGame();
        //}
    }

    public void OnClick_DisconnectToMenu()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GoBackToMenu", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void GoBackToMenu()
    {
        PhotonNetwork.DestroyAll();
        SceneManager.LoadScene(0);
    }
    #endregion


    public void RestartMultiplayerGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("LoadGame", RpcTarget.All);
        }
    }

    [PunRPC]
    private void LoadGame()
    {
        Debug.Log("MASTER CLIENT _ Restart Game");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(1);
        launchingGame = true;
    }
}
