using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BallSetup : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("BallSetup");
        BallManager.instance.SetupBall(gameObject);
        GameManager.Instance.ReadyCheck(GameManager.Instance.BallFirstSpawn);
    }
}
