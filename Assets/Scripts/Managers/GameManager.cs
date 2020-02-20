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

    [Header("Timer Settings")]
    public float timerSpeedModifier = 1f;
    [HideInInspector] public GUITimerData timerData;
    [HideInInspector] public float currentTimer;
    [HideInInspector] public float timeMax;

    private int seconds;
    private int mSeconds;

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
            SelectionLevel(CampaignLevel.Instance.levelSelected);
        }
        else
        {
            if (gameMod == GameMod.GAMEPLAY)
            {
                Debug.Log(MultiLevel.Instance.levelIndex);
                SelectionLevel(MultiLevel.Instance.levelIndex);
            }
        }
        UpdateTimeText();
    }

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


    //////////////// TIMER ////////////////

    void Update()
    {
        if (gameMod == GameMod.GAMEPLAY && isGameStart && !isTimerStopped && !hasLost)
        {
            UpdateTimer();
        }

        //Debug.Log(levelIndex);
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
        BallEventManager.instance.OnCollisionWithRacket -= StartTheGame;
    }

    public void EndOfTheGame()
    {
        LevelManager.instance.playersHUD.EnableScoreScreen();
        isGameStart = false;
        LevelManager.instance.CleanWalls();
        BallManager.instance.DespawnTheBall();
    }

    public bool GetGameStatus()
    {
        return isGameStart;
    }

    private void UpdateTimer()
    {
        if (currentTimer >= 0)
        {
            currentTimer -= Time.deltaTime * timerSpeedModifier;
        }

        if(currentTimer < 0)
        {
            isTimerStopped = true;
            hasLost = true;

            EndOfTheGame();
            //GameOver STATE
        }

        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        if (currentTimer < 0)
        {
            timerData.UpdateText("00:00");
            timerData.FillImage(0);
        }
        else
        {
            seconds = (int)(currentTimer / 60);
            mSeconds = (int)(currentTimer - (seconds * 60));

            if (seconds < 10)
            {
                if (mSeconds < 10)
                {
                    timerData.UpdateText("0" + seconds + ":" + "0" + mSeconds);
                }
                else
                {
                    timerData.UpdateText("0" + seconds + ":" + mSeconds);
                }
            }
            else
            {
                if (mSeconds < 10)
                {
                    timerData.UpdateText(seconds + ":" + "0" + mSeconds);
                }
                else
                {
                    timerData.UpdateText(seconds + ":" + mSeconds);
                }
            }

            timerData.FillImage(currentTimer / timeMax);
        }
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



    public void SelectionLevel(int selection){
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }

    public void SelectionLevel(LevelsScriptable selection)
    {
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }
}
