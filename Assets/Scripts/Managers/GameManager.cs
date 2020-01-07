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
    public GUITimerData timerData;
    public float currentTimer;
    public float timerSpeedModifier;
    public float timeMax;

    private int seconds;
    private int mSeconds;
    private bool isGameStart = false;

    [Header("Offline Mode")]
    public bool offlineMode = false;

    void Start()
    {
        if (offlineMode)
        {
            PhotonNetwork.OfflineMode = true;
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

    void Update()
    {
        if (isGameStart)
        {
            UpdateTimer();
        }
    }

    public void StartTheGame()
    {
        isGameStart = true;
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
}
