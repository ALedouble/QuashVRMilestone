using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
        //QPlayerManager.instance.SetPlayer(PhotonNetwork.Instantiate(prefabPlayer.name, prefabPlayer.transform.position, Quaternion.identity, 0), 1);
      //  PhotonNetwork.Instantiate(prefabBall.name, prefabBall.transform.position, Quaternion.identity, 0);

        Instance = this;
        
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient){
            QPlayerManager.instance.SetPlayer(PhotonNetwork.Instantiate(prefabPlayer.name, spawnJ1.transform.position, Quaternion.identity, 0) as GameObject, 1);
        }
        else{
            QPlayerManager.instance.SetPlayer(PhotonNetwork.Instantiate(prefabPlayer.name, spawnJ2.transform.position, Quaternion.identity, 0) as GameObject, 1);
        }
       

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
