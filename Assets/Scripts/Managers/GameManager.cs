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

    [Header("BallSpawnSettings")]
    public float ballSpawnDelay;

    private bool isReady = false;
    private bool isGameStart = false;
    public bool IsReady { get => isReady; }
    public bool IsGameStarted { get => isGameStart; }
    PhotonView photonView;

    public bool isTimerStopped = false;
    public bool hasLost = false;

    [Header("Offline Mode")]
    public bool offlineMode = false;
    
    [HideInInspector]
    public int levelIndex;

    void Awake()
    {
        BrickBehaviours.ResetBrickCount();
        Instance = this;
        SetupOfflineMod();
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        InstantiatePlayers();

        SpawnLevel();
        
        InstanciateBall();
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
              //  photonView.RPC("SelectionLevelRPC", RpcTarget.All, MultiLevel.Instance.levelIndex);

                 SelectionLevelRPC(MultiLevel.Instance.levelIndex);
            }
        }
    }

    #region Ball
    public void InstanciateBall()                   //Rename
    {
        if(gameMod == GameMod.GAMEPLAY /*&& PhotonNetwork.IsMasterClient*/)
        {
            StartCoroutine(InstantiateBallWithDelay());
            if(offlineMode || PhotonNetwork.IsMasterClient)
                SynchronizeStart();
        }
    }
   
    private IEnumerator InstantiateBallWithDelay()
    {
        yield return new WaitForFixedUpdate();

        BallManager.instance.InitializeBall();
    }

    #endregion

    private void SynchronizeStart()
    {
        StartCoroutine(DelaySynchStart());
    }

    private IEnumerator DelaySynchStart()
    {
        yield return new WaitForSeconds(ballSpawnDelay);
        BallManager.instance.SpawnTheBall();

        if (offlineMode)
            StartBrickMovement();
        else if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartBrickMovement", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartBrickMovement()
    {
        isReady = true;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        isGameStart = true;

        TimeManager.Instance.OnTimerEnd += EndOfTheGame;
        TimeManager.Instance.StartTimer();

        BallEventManager.instance.OnCollisionWithRacket -= StartTheGame;
    }

    public void EndOfTheGame()
    {
        TimeManager.Instance.OnTimerEnd -= EndOfTheGame;

        TimeManager.Instance.StopTimer();
        LevelManager.instance.playersHUD.EnableScoreScreen();
        isGameStart = false;
        LevelManager.instance.CleanWalls();
        BallManager.instance.DespawnTheBall();
    }

    public bool GetGameStatus()
    {
        return isGameStart;
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


    public void SelectionLevelRPC(int selection){
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    public void SelectionLevel(LevelsScriptable selection)
    {
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }
}
