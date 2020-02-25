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
    public bool HasLost { get; private set; }                                                                            //A passer en property

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
    }

    void Start()
    {
        if(offlineMode || PhotonNetwork.IsMasterClient)
        {
            InstantiatePlayers();
            SpawnLevel();

            isReadyToContinue = true;
            ReadyCheck(InstanciateBall);

            ReadyCheck(StartBrickMovement);

            ReadyCheck(BallFirstSpawn);
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
            SelectionLevelRPC(CampaignLevel.Instance.levelSelected);
        }
        else
        {
            if (gameMod == GameMod.GAMEPLAY)
            {
                Debug.Log(MultiLevel.Instance.levelIndex);
                photonView.RPC("SelectionLevelRPC", RpcTarget.All, MultiLevel.Instance.levelIndex);

                // SelectionLevel(MultiLevel.Instance.levelIndex);
            }
        }
    }

    
    private void InstanciateBall()
    {
        if(gameMod == GameMod.GAMEPLAY)
        {
            BallManager.instance.InstantiateBall();
        }
    }

    [PunRPC]
    private void StartBrickMovement()
    {
        IsBrickFreeToMove = true;
    }

    private void BallFirstSpawn()
    {
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
        IsGameStarted = false;

        LevelManager.instance.playersHUD.EnableScoreScreen();

        LevelManager.instance.CleanWalls();
        BallManager.instance.DespawnTheBall();
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


    [PunRPC]
    public void SelectionLevelRPC(int selection){
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    public void SelectionLevel(LevelsScriptable selection)
    {
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    private void ReadyCheck(Action nextAction = null)
    {
        if (nextAction != null)
        {
            ReadyCheckDelegateQueue.Enqueue(nextAction);
        }

        Action ReadyCheckDelegate;

        if (offlineMode && ReadyCheckDelegateQueue.Count > 0)
        {
            ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
            ReadyCheckDelegate();
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            if (isReadyToContinue && ReadyCheckDelegateQueue.Count > 0)
            {
                isReadyToContinue = false;
                ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
                ReadyCheckDelegate();                                                               // Is there too much latenty?
                photonView.RPC("ReadyCheck", RpcTarget.Others, ReadyCheckDelegate.Method.Name);             // Replace par ReadyCheck
            }
            else if (!isReadyToContinue)
            {
                isReadyToContinue = true;
                ReadyCheck();
            }
            else
            {
                Debug.LogError("GameManager.ReadyCheck : Unexpected behaviour!");
            }
        }
        else
        {
            ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
            ReadyCheckDelegate();
            photonView.RPC("ReadyCheck", RpcTarget.MasterClient);
        }
    }

    private void ReadyCheck(string methodName)
    {
        MethodInfo methodInfo = this.GetType().GetMethod(methodName);
        if (methodInfo != null)
            ReadyCheck((Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo));
        else
            Debug.LogError("GameManager.ReadyCheck(string) : method " + methodName + " doesn't exist!");
    }
}
