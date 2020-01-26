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

    [Header("Timer Settings")]
    public float timerSpeedModifier = 1f;
    [HideInInspector] public GUITimerData timerData;
    [HideInInspector] public float currentTimer;
    [HideInInspector] public float timeMax;

    private int seconds;
    private int mSeconds;
    private bool isGameStart = false;
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
        Instance = this;
        SetupOfflineMod();
    }

    void Start()
    {
        

        if (offlineMode)
        {
            SelectionLevel(CampaignLevel.Instance.levelSelected);
        }

            

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
        if (!offlineMode)
        {
            if (gameMod == GameMod.GAMEPLAY)
            {
                Debug.Log(MultiLevel.Instance.levelIndex);
                SelectionLevel(MultiLevel.Instance.levelIndex);
            }
        }
    }

    public void InstanciateBall()
    {
        if(gameMod == GameMod.GAMEPLAY)
            StartCoroutine(InstantiateBallWithDelay());
    }
   
    private IEnumerator InstantiateBallWithDelay()
    {
        yield return new WaitForFixedUpdate();

        BallManager.instance.InitializeBall();
        BallEventManager.instance.OnCollisionWithRacket += StartTheGame;
        BallManager.instance.SpawnTheBall();
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
        if (isGameStart && !isTimerStopped && !hasLost)
        {
            UpdateTimer();
        }

        Debug.Log(levelIndex);
    }

    [PunRPC]
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

            seconds = (int)(currentTimer / 60);
            mSeconds = (int)(currentTimer - (seconds * 60));
        }

        if (currentTimer < 0)
        {
            timerData.UpdateText("00:00");
            timerData.FillImage(0);

            isTimerStopped = true;
            hasLost = true;

            EndOfTheGame();
            //GameOver STATE
        }
        else
        {
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

    public void ReturnMenu()
    {   
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    public void SelectionLevel(int selection){
        //LevelManager.instance.ConfigDistribution(selection);
        LevelManager.instance.StartLevelInitialization(selection);
    }
}
