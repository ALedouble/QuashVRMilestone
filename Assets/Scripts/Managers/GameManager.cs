using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMod
{
    MENU = 0,
    GAMEPLAY = 1
}

public class GameManager : MonoBehaviour
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

    [Header("Offline Mode")]
    public bool offlineMode = false;

    public bool IsBrickFreeToMove { get; private set; }                                                                                                                     
    public bool IsGameStarted { get; private set; }
    public bool HasLost { get => (IsGameStarted) && (TimeManager.Instance.CurrentTimer <= 0); }

    [HideInInspector]
    public int levelIndex;

    private bool isReadyToContinue = false;
    private Queue<Action> ReadyCheckDelegateQueue;

    PhotonView photonView;


    void Awake()
    {
        BrickBehaviours.ResetBrickCount();
        Instance = this;
        SetupOfflineMod();
        photonView = GetComponent<PhotonView>();
        ReadyCheckDelegateQueue = new Queue<Action>();
    }

    void Start()
    {
        InstantiatePlayers();
        SpawnLevel();

        if (offlineMode || PhotonNetwork.IsMasterClient)
        {
            isReadyToContinue = true;
            ReadyCheck(InstanciateBall);

            ReadyCheck(StartBrickMovement);
        }
    }

    private void SetupOfflineMod()
    {
        if (offlineMode)
        {
            PhotonNetwork.Disconnect();
            //Debug.Log(PhotonNetwork.OfflineMode);
            // PhotonNetwork.OfflineMode = true;
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
        if (!offlineMode)
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

    private void SpawnLevel()
    {
        if (offlineMode)
        {
            SelectionLevelMulti(CampaignLevel.Instance.levelSelected);
        }
        else
        {
            if (gameMod == GameMod.GAMEPLAY)
            {
                Debug.Log(MultiLevel.Instance.levelIndex);
              //  photonView.RPC("SelectionLevelRPC", RpcTarget.All, MultiLevel.Instance.levelIndex);

                 SelectionLevelMulti(MultiLevel.Instance.levelIndex);
            }
        }
    }

    
    public void InstanciateBall()
    {
        Debug.Log("GameManager Instanciate ball");
        if(gameMod == GameMod.GAMEPLAY)
        {
            BallManager.instance.InstantiateBall();
        }
    }

    [PunRPC]
    public void StartBrickMovement()
    {
        IsBrickFreeToMove = true;
    }

    public void BallFirstSpawn()
    {
        Debug.Log("BallFirstSpawn");
        BallEventManager.instance.OnCollisionWithRacket += StartTheGame;
        BallManager.instance.BallFirstSpawn();
    }

    public Transform[] GetPlayerSpawn()
    {
        return playerSpawn;
    }

    
    public void StartTheGame()
    {
        if(offlineMode)
        {
            StartTheGameRPC();
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartTheGameRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartTheGameRPC()
    {
        IsGameStarted = true;

        TimeManager.Instance.OnTimerEnd += EndOfTheGame;
        TimeManager.Instance.StartTimer();

        BallEventManager.instance.OnCollisionWithRacket -= StartTheGame;
    }

    public void EndOfTheGame()
    {
        TimeManager.Instance.OnTimerEnd -= EndOfTheGame;
        TimeManager.Instance.StopTimer();

        PlayerInputManager.instance.SetInputMod(InputMod.MENU);

        LevelManager.instance.playersHUD.EnableScoreScreen();

        LevelManager.instance.CleanWalls();
        BallManager.instance.DespawnTheBall();

        

        //If the player who WINS is ALONE
        if(!HasLost && offlineMode)
        {
            JSON.instance.SubmitDATA(LevelManager.instance.currentLevel, (int)ScoreManager.Instance.score[0], ScoreManager.Instance.playersMaxCombo[0], (int)TimeManager.Instance.CurrentTimer);
        }
    }

    public void RestartScene()
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


    public void SelectionLevelMulti(int selection){
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    public void SelectionLevel(LevelsScriptable selection)
    {
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    [PunRPC]
    public void ReadyCheck(Action nextAction = null)
    {
        if (nextAction != null)
        {
            ReadyCheckDelegateQueue.Enqueue(nextAction);
        }

        Action ReadyCheckDelegate;
        if(ReadyCheckDelegateQueue.Count > 0)
        {
            if (offlineMode)
            {
                ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
                ReadyCheckDelegate();
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                if (isReadyToContinue)
                {
                    isReadyToContinue = false;
                    ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
                    ReadyCheckDelegate();                                                               // Is there too much latenty?
                    photonView.RPC("ReadyCheck", RpcTarget.Others, ReadyCheckDelegate.Method.Name);             // Replace par ReadyCheck
                }
            }
            else
            {
                ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
                ReadyCheckDelegate();
                photonView.RPC("ResumeReadyCheck", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    public void ResumeReadyCheck()
    {
        isReadyToContinue = true;
        ReadyCheck();
    }

    [PunRPC]
    private void ReadyCheck(string methodName)
    {
        MethodInfo methodInfo = this.GetType().GetMethod(methodName);
        if (methodInfo != null)
            ReadyCheck((Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo));
        else
            Debug.LogError("GameManager.ReadyCheck(string) : method " + methodName + " doesn't exist!");
    }

    public void SendResumeRPC()
    {
        photonView.RPC("ResumeReadyCheck", RpcTarget.MasterClient);
    }

    public void DisconnectToMenu(){
        if(gameMod == GameMod.GAMEPLAY)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1){
                PhotonNetwork.DestroyAll();
                SceneManager.LoadScene(0);
            }
        }
    }

    private void Update() {
        DisconnectToMenu();
    }
}
