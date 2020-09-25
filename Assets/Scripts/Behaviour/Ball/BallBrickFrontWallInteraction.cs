using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallBrickFrontWallInteraction : MonoBehaviour
{
    public float depthVelocity;
    public float xAcceleration;

    public float bounceDelay = 0.45f;

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
        targetSelector = GetComponent<ITargetSelector>();

        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        BallMultiplayerBehaviour.Instance.ReturnSwitchActions += StartReturn;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ( !BallManager.instance.IsBallPaused && (other.gameObject.tag == "FrontWall" || other.gameObject.tag == "Brick") )
        {
            if(GameManager.Instance.offlineMode)
            {
                BallEventManager.instance.OnBallCollision(other.gameObject.tag, other);

                PlayFeedback(other.gameObject.tag, other.GetContact(0).point);

                ReturnInteration();
                
            }
            else
            {
                if(BallMultiplayerBehaviour.Instance.IsBallOwner)
                {
                    BallEventManager.instance.OnBallCollision("FrontWall", other);

                    SendFeedbackRPC(other.gameObject.tag, other.GetContact(0).point);

                    BallMultiplayerBehaviour.Instance.HandOverBallOwnership(BallOwnershipSwitchType.Return);
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
        photonView.RPC("PlayFeedback", RpcTarget.All, tag, contactPoint);
    }

    [PunRPC]
    private void PlayFeedback(string tag, Vector3 contactPoint)
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
        ReturnInteration();
    }

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

    private IEnumerator IgnoreCollision()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), true);

        float timer = 0f;
        float duration = bounceDelay + (Mathf.Abs(transform.position.z - targetSelector.GetTargetPlayerPosition().z / depthVelocity) / 2f);
        while (timer < duration)
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

    #region MidWallStatus

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

    #endregion
}
