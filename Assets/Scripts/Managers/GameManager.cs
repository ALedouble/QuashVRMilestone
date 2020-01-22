using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }
    #endregion

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public GameObject racketPrefab;

    public GameObject spawnJ1;
    public GameObject spawnJ2;

    [Header("Timer Settings")]
    public float timerSpeedModifier = 1f;
    [HideInInspector] public GUITimerData timerData;
    [HideInInspector] public float currentTimer;
    [HideInInspector] public float timeMax;

    private int seconds;
    private int mSeconds;
    private bool isGameStart = false;

    public bool isTimerStopped = false;
    public bool hasLost = false;

    [Header("Offline Mode")]
    public bool offlineMode = false;
    
    void Start()
    {
        if (offlineMode)
        {
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            PhotonNetwork.OfflineMode = false;
        }
        
        if(!PhotonNetwork.OfflineMode)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                QPlayerManager.instance.SetLocalPlayer(PhotonNetwork.Instantiate(playerPrefab.name, spawnJ1.transform.position, Quaternion.identity, 0) as GameObject);

                RacketManager.instance.SetLocalRacket(PhotonNetwork.Instantiate("RacketPlayer", Vector3.zero, Quaternion.identity) as GameObject);

                //  PhotonNetwork.Instantiate(prefabBall.name, prefabBall.transform.position, Quaternion.identity, 0);
            }
            else
            {
                QPlayerManager.instance.SetLocalPlayer(PhotonNetwork.Instantiate(playerPrefab.name, spawnJ2.transform.position, Quaternion.identity, 0) as GameObject);

                RacketManager.instance.SetLocalRacket(PhotonNetwork.Instantiate("RacketPlayer", Vector3.zero, Quaternion.identity) as GameObject);
            }
        }
        else
        {
            QPlayerManager.instance.SetLocalPlayer(Instantiate(playerPrefab, spawnJ2.transform.position, Quaternion.identity));

            RacketManager.instance.SetLocalRacket(Instantiate(racketPrefab, Vector3.zero, Quaternion.identity));
        }
        

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }





    //////////////// TIMER ////////////////

    void Update()
    {
        if (isGameStart && !isTimerStopped && !hasLost)
        {
            UpdateTimer();
        }

        if (PhotonNetwork.PlayerList.Length == 0)
        {
            //Condition de déconnexion de la room Multi et retour dans le menu.
            Debug.Log("oui");
        }
        else
        {
            Debug.Log("non");
        }
    }

    public void StartTheGame()
    {
        isGameStart = true;
    }

    public void EndOfTheGame()
    {
        LevelManager.instance.playersHUD.EnableScoreScreen();
        isGameStart = false;
        LevelManager.instance.CleanWalls();
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


    [PunRPC]
    public void Restart(int view)
    {
        Scene sceneLoaded = SceneManager.GetActiveScene();

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(sceneLoaded.buildIndex);
    }

    public void ReturnMenu()
    {   
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
