using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Lumin;

public class BallSetup : MonoBehaviour
{
    private PhotonView photonview;

    private void Awake()
    {
        photonview = gameObject.GetComponent<PhotonView>();
    }

    private void Start()
    {
        //Debug.Log("BallSetup.Start");
        
        if(!GameManager.Instance.offlineMode)
        {
            gameObject.SetActive(false);
            GameManager.Instance.SendResumeRPC();
        }
            
    }

    [PunRPC]
    public void SetupBall()
    {
        //Ajouter la mise au point de la couleur?
        //GameManager.Instance.BallFirstSpawn();
        //Debug.Log("SetupBall");
        BallManager.instance.SetupBall(gameObject);
    }

    public void SendSetupBallRPC()
    {
        photonview.RPC("SetupBall", RpcTarget.All);
    }
}
