﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallBackWallInteraction : MonoBehaviour
{
    private BallPhysicBehaviour ballPhysicBehaviour;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "BackWall")
        {
            Debug.Log("BackWall Collision");
            if(GameManager.Instance.offlineMode)
            {
                BallManager.instance.LoseBall();
                ScoreManager.Instance.ResetCombo((int)BallManager.instance.GetPlayerWhoLostTheBall());
            }
            else if (BallMultiplayerBehaviour.Instance.IsBallOwner)
            {
                BallManager.instance.LoseBall();
                //ScoreManager.Instance.ResetCombo((int)BallManager.instance.GetPlayerWhoLostTheBall());
            }
            else
            {
                ballPhysicBehaviour.FreezeBall();
            }
                
            BallEventManager.instance.OnBallCollision("BackWall", other);
        }
    }
}
