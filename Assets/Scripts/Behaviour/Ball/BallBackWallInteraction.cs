using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallBackWallInteraction : MonoBehaviour
{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "BackWall")
        {
            BallManager.instance.LoseBall();

            SendBallCollisionEvent("BackWall");
        }
    }

    #region CollisionEvent

    //Need Rework!!!
    private void SendBallCollisionEvent(string tag)
    {
        if (GameManager.Instance.offlineMode)
        {
            OnBallCollisionRPC(tag);
        }
        else if (tag == "Racket")
        {
            //photonView.RPC("OnBallCollisionRPC", RpcTarget.All, tag);
            BallEventManager.instance.OnBallCollision(tag);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("OnBallCollisionRPC", RpcTarget.All, tag);
            }
        }
    }

    [PunRPC]
    private void OnBallCollisionRPC(string tag)
    {
        BallEventManager.instance.OnBallCollision(tag);
    }

    #endregion

}
