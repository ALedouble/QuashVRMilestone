using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BallSetup : MonoBehaviour
{
    private void Awake()
    {
        BallManager.instance.SetupBall(gameObject);
    }
}
