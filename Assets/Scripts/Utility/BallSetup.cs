using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Lumin;

public class BallSetup : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("BallSetup");
        BallManager.instance.SetupBall(gameObject);
        if (GameManager.Instance.offlineMode || PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ReadyCheck BallFirstSpawn!");
            GameManager.Instance.ReadyCheck(GameManager.Instance.BallFirstSpawn);
        } 
    }
}
