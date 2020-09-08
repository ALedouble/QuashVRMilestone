using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallFrontWallInteraction : MonoBehaviour
{
    public float depthVelocity;
    public float xAcceleration;

    private ITargetSelector targetSelector;
    private NoBounceMagicReturn nBMagicReturn;

    private BallPhysicBehaviour ballPhysicBehaviour;
    private BallInfo ballInfo;

    private Coroutine IgnoreCollisionCoroutine;

    private PhotonView photonView;

    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();
        ballInfo = GetComponent<BallInfo>();
        nBMagicReturn = new NoBounceMagicReturn(depthVelocity, ballPhysicBehaviour.baseGravity, xAcceleration);

        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "FrontWall")
        {
            ReturnInteration();
            IgnoreCollisionCoroutine = StartCoroutine(IgnoreCollision());
            // Sound Magnitude TO BE FIX !!!
            AudioManager.instance.PlaySound("FrontWallHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude);

            SendBallCollisionEvent("FrontWall");
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

    #region ReturnInteraction

    private void ReturnInteration()
    {
        RandomReturnWithoutBounce();
        SetMidWallStatus(false);
    }

    private void RandomReturnWithoutBounce()
    {
        Vector3 targetPosition = targetSelector.GetNewTargetPosition();
        Vector3 newVelocity = nBMagicReturn.CalculateNewVelocity(transform.position, targetPosition);

        ballPhysicBehaviour.ApplyNewVelocity(newVelocity * ballPhysicBehaviour.globalSpeedMultiplier, transform.position, (int)SpeedState.SLOW, true);
    }

    private void SetMidWallStatus(bool isCollidable)
    {
        if (GameManager.Instance.offlineMode)
        {
            if (isCollidable)
                ActivateMidWall();
            else
                DisactivateMidWall();
        }
        else
        {
            if (isCollidable)
                photonView.RPC("ActivateMidWall", RpcTarget.All);
            else
                photonView.RPC("DisactivateMidWall", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ActivateMidWall()
    {
        if (LevelManager.instance.numberOfPlayers > 1)
        {
            //LevelManager.instance.midCollider.enabled = true;
            LevelManager.instance.midCollider.gameObject.SetActive(true);

        }

    }

    [PunRPC]
    private void DisactivateMidWall()
    {
        if (LevelManager.instance.numberOfPlayers > 1)
        {
            //LevelManager.instance.midCollider.enabled = false;
            LevelManager.instance.midCollider.gameObject.SetActive(false);
        }
    }

    private IEnumerator IgnoreCollision()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), true);

        float timer = 0f;
        while (timer < 0.1f)
        {
            yield return new WaitForFixedUpdate();
            if (!BallManager.instance.IsBallPaused)
            {
                timer += Time.fixedDeltaTime;
            }
        }

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), false);
    }

    #endregion
}
