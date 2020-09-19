using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallFrontWallInteraction : MonoBehaviour
{
    public float depthVelocity;
    public float xAcceleration;

    public float bounceDelay = 0.45f;

    private ITargetSelector targetSelector;
    private NoBounceMagicReturn nBMagicReturn;

    private BallPhysicBehaviour ballPhysicBehaviour;
    private BallInfo ballInfo;
    private BallPhysicInfo ballPhysicInfo;

    private Coroutine IgnoreCollisionCoroutine;

    private PhotonView photonView;

    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();
        ballInfo = GetComponent<BallInfo>();
        ballPhysicInfo = GetComponent<BallPhysicInfo>();

        nBMagicReturn = new NoBounceMagicReturn(depthVelocity, ballPhysicBehaviour.baseGravity, xAcceleration);
        targetSelector = GetComponent<ITargetSelector>();

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

            BallEventManager.instance.OnBallCollision("FrontWall");

            ballPhysicInfo.IsOnFrontWallCollisionFrame = true;
        }
    }

    #region ReturnInteraction

    private void ReturnInteration()
    {
        StartCoroutine(RandomReturnWithoutBounce());
        IgnoreCollisionCoroutine = StartCoroutine(IgnoreCollision());
        SetMidWallStatus(false);
    }

    private IEnumerator RandomReturnWithoutBounce()
    {
        Vector3 targetPosition = targetSelector.GetNewTargetPosition();
        Vector3 newVelocity = nBMagicReturn.CalculateNewVelocity(transform.position, targetPosition);

        ballPhysicBehaviour.SetGravityState(false);
        ballPhysicBehaviour.ApplyNewVelocity(Vector3.zero);

        float timer = 0f;
        while(timer < bounceDelay)
        {
            if (!GameManager.Instance.IsGamePaused)
                timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ballPhysicBehaviour.SetGravityState(true);
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
        while (timer < (bounceDelay + 0.1f))
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
