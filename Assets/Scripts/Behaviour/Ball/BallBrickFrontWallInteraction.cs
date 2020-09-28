using System.Collections;
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

    //[PunRPC]
    //private void BrickWallCollision(string tag, Vector3 contactPoint)
    //{

    //}

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
        ReturnInteration();
    }

    private void ReturnInteration()
    {
        StartCoroutine(RandomReturnWithoutBounce());
        IgnoreCollisionCoroutine = StartCoroutine(OwnerCollisionProtection());
        
        MidWallManager.Instance.SetMidWallStatus(false);
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

    #region Collision

    private IEnumerator OwnerCollisionProtection()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), true);

        float timer = 0f;
        float duration = bounceDelay + (Mathf.Abs(transform.position.z - targetSelector.GetTargetPlayerPosition().z / depthVelocity) / 2f);
        photonView.RPC("FolowerCollisionProtection", RpcTarget.Others, duration / 1.5f);

        Debug.Log("Ignore collsion duration : " + duration);
        while (timer < duration)
        {
            yield return new WaitForFixedUpdate();
            if (!BallManager.instance.IsBallPaused)
            {
                timer += Time.fixedDeltaTime;
            }
        }
        Debug.Log("Ignore collsion timelaps : " + timer);
        Debug.Log("Reactivate floor collision");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), false);
    }

    [PunRPC]
    private void FolowerCollisionProtection(float delay)
    {
        StartCoroutine(DelayedColliderActivationCoroutine(delay));
    }

    private IEnumerator DelayedColliderActivationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        BallManager.instance.BallPhysicBehaviour.ActivateCollider();
    }

    #endregion

    #endregion

}
