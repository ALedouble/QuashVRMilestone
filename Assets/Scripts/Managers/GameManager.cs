using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject prefabPlayer;

    public GameObject spawnJ1;
    public GameObject spawnJ2;

    

   

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {
        
        if (PhotonNetwork.IsMasterClient){
            QPlayerManager.instance.SetLocalPlayer(PhotonNetwork.Instantiate(prefabPlayer.name, spawnJ1.transform.position, Quaternion.identity, 0) as GameObject);

            RacketManager.instance.SetLocalRacket(PhotonNetwork.Instantiate("RacketPlayer", Vector3.zero, Quaternion.identity) as GameObject);

            //  PhotonNetwork.Instantiate(prefabBall.name, prefabBall.transform.position, Quaternion.identity, 0);
        }
        else 
        {
            QPlayerManager.instance.SetLocalPlayer(PhotonNetwork.Instantiate(prefabPlayer.name, spawnJ2.transform.position, Quaternion.identity, 0) as GameObject);
           
            RacketManager.instance.SetLocalRacket(PhotonNetwork.Instantiate("RacketPlayer", Vector3.zero, Quaternion.identity) as GameObject);
                
        }

    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
