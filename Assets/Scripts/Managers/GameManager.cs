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

    void Awake()
    {
        Instance = this;
    }
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
    PhotonView photonView;

    public bool isTimerStopped = false;
    public bool hasLost = false;

    [Header("Offline Mode")]
    public bool offlineMode = false;
    
    [HideInInspector]
    public int levelIndex;
    
    void Start()
    {
        
        if (offlineMode)
        {
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            PhotonNetwork.OfflineMode = false;

            if(gameMod == GameMod.GAMEPLAY){
                photonView.RPC("SyncLevelRPC", RpcTarget.All);
            }
        }
        
        if(!PhotonNetwork.OfflineMode)
        {
            
            if (PhotonNetwork.IsMasterClient)
            {
                QPlayerManager.instance.SetLocalPlayer(PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn[0].position, Quaternion.identity, 0) as GameObject);
                
                if(gameMod == GameMod.GAMEPLAY)
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
                RacketManager.instance.SetLocalRacket(Instantiate(racketPrefab, Vector3.zero, Quaternion.identity));
        }
        

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;

        
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [PunRPC]
    public void SyncLevelRPC(){
        SelectionLevel(MultiLevel.Instance.levelIndex);
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

    public void StartTheGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitForFixedUpdate();

        isGameStart = true;

        if (BallManager.instance.isBallInstatiated)
        {
            BallManager.instance.SetupBall();
            BallManager.instance.SpawnTheBall();
        }
            
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
        LevelManager.instance.ConfigDistribution(selection);
    }
}
