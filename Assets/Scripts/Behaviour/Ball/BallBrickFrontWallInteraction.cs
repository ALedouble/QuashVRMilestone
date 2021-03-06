﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallBrickFrontWallInteraction : MonoBehaviour
{
    public float depthVelocity;
    public float xAcceleration;

    public float bounceDelay;

    private ITargetSelector targetSelector;
    private NoBounceMagicReturn nBMagicReturn;

    private BallPhysicBehaviour ballPhysicBehaviour;
    private BallInfo ballInfo;

    private Coroutine ownerCollisionProtectionCoroutine;
    private Coroutine followerCollisionProtectionCoroutine;

    private PhotonView photonView;

    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();
        ballInfo = GetComponent<BallInfo>();

        nBMagicReturn = new NoBounceMagicReturn(depthVelocity, ballPhysicBehaviour.baseGravity, xAcceleration);
        targetSelector = GetComponent<ITargetSelector>();

        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        BallMultiplayerBehaviour.Instance.ReturnSwitchActions += StartReturn;
        BallManager.instance.OnBallReset += Reset;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ( !BallManager.instance.IsBallPaused && (other.gameObject.tag == "FrontWall" || other.gameObject.tag == "Brick") )
        {
            if(GameManager.Instance.offlineMode)
            {
                BallEventManager.instance.OnBallCollision(other.gameObject.tag, other);

                PlayBrickFrontWallCollisionFeedback(other.gameObject.tag, other.GetContact(0).point);

                ReturnInteration();
            }
            else
            {
                if(BallMultiplayerBehaviour.Instance.IsBallOwner)
                {
                    BallEventManager.instance.OnBallCollision(other.gameObject.tag, other);

                    SendFeedbackRPC(other.gameObject.tag, other.GetContact(0).point);

                    BallMultiplayerBehaviour.Instance.HandOverBallOwnership(BallOwnershipSwitchType.Return);

                    BallManager.instance.BallPhysicBehaviour.FreezeBall();
                }
                else
                {
                    ballPhysicBehaviour.FreezeBall();
                }
            }
            
        }
    }

    private void SendFeedbackRPC(string tag, Vector3 contactPoint)
    {
        photonView.RPC("PlayBrickFrontWallCollisionFeedback", RpcTarget.All, tag, contactPoint);
    }

    [PunRPC]
    private void PlayBrickFrontWallCollisionFeedback(string tag, Vector3 contactPoint)
    {
        switch(tag)
        {
            case "FrontWall":
                AudioManager.instance.PlaySound("FrontWallHit", contactPoint, ballPhysicBehaviour.LastVelocity.magnitude);
                break;
            case "Brick":
                AudioManager.instance.PlaySound("BrickExplosion", contactPoint, ballPhysicBehaviour.LastVelocity.magnitude);
                break;

        }
    }

    #region ReturnInteraction

    public void StartReturn()
    {
        BallMultiplayerBehaviour.Instance.ExitReturnCase();
        ReturnInteration();
    }

    private void ReturnInteration()
    {
        ownerCollisionProtectionCoroutine = StartCoroutine(OwnerCollisionProtection());
        
        StartCoroutine(RandomReturnWithoutBounce());

        MidWallManager.Instance.SetMidWallStatus(false);
    }

    private IEnumerator RandomReturnWithoutBounce()
    {
        BallManager.instance.BallPhysicBehaviour.FreezeBall();

        yield return new WaitForFixedUpdate();

        BallManager.instance.BallPhysicBehaviour.UnFreezeBall();

        Vector3 targetPosition = targetSelector.GetNewTargetPosition();
        Vector3 newVelocity = nBMagicReturn.CalculateNewVelocity(transform.position, targetPosition);

        ballPhysicBehaviour.SetGravityState(false);
        ballPhysicBehaviour.PhysicRelatedVelocityUpdate(Vector3.zero);

        float timer = 0f;
        while(timer < bounceDelay)
        {
            if (!GameManager.Instance.IsGamePaused)
                timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ballPhysicBehaviour.SetGravityState(true);
        ballPhysicBehaviour.OverrideRawVelocity(newVelocity, (int)SpeedState.SLOW, true);
    }

    #endregion

    #region Collision

    private IEnumerator OwnerCollisionProtection()
    {
        SetCollisionState(false);

        BallManager.instance.BallPhysicBehaviour.ActivateCollider();

        float timer = 0f;
        float duration = bounceDelay + (Mathf.Abs(transform.position.z - targetSelector.GetTargetPlayerPosition().z / depthVelocity) / 2f);
        
        if(!GameManager.Instance.offlineMode)
            photonView.RPC("FollowerCollisionProtection", RpcTarget.Others, duration / 1.5f);

        //Debug.Log("Ignore collsion duration : " + duration);
        while (timer < duration)
        {
            yield return new WaitForFixedUpdate();
            if (!BallManager.instance.IsBallPaused)
            {
                timer += Time.fixedDeltaTime;
            }
        }
        //Debug.Log("Ignore collsion timelaps : " + timer);

        //Debug.Log("Reactivate floor collision");
        SetCollisionState(true);
    }

    [PunRPC]
    private void FollowerCollisionProtection(float delay)
    {
        followerCollisionProtectionCoroutine = StartCoroutine(DelayedColliderActivationCoroutine(delay));
    }

    private IEnumerator DelayedColliderActivationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        BallManager.instance.BallPhysicBehaviour.ActivateCollider();
    }

    private void SetCollisionState(bool active)
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), !active);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), !active);
        //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("FrontWall"), !active);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Brick"), !active);
    }

    #endregion

    public void Reset()
    {
        if (ownerCollisionProtectionCoroutine != null)
            StopCoroutine(ownerCollisionProtectionCoroutine);

        if (followerCollisionProtectionCoroutine != null)
            StopCoroutine(followerCollisionProtectionCoroutine);

        SetCollisionState(true);
    }
}
