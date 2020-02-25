using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BallSetup : MonoBehaviour
{
    private void Start()
    {
        BallManager.instance.SetupBall(gameObject);
        GameManager.Instance.ReadyCheck(GameManager.Instance.BallFirstSpawn);
    }
}
