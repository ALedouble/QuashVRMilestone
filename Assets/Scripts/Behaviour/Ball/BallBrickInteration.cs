using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallBrickInteration : MonoBehaviour
{
    public float depthVelocity;
    public float xAcceleration;

    public float bounceDelay = 0.45f;

    private ITargetSelector targetSelector;
    private NoBounceMagicReturn nBMagicReturn;

    private BallPhysicBehaviour ballPhysicBehaviour;

    private PhotonView photonView;

    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();

        nBMagicReturn = new NoBounceMagicReturn(depthVelocity, ballPhysicBehaviour.baseGravity, xAcceleration);
        targetSelector = GetComponent<ITargetSelector>();

        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "Brick")
        {
            if(GameManager.Instance.offlineMode)
            {
                AudioManager.instance.PlaySound("BrickExplosion", other.GetContact(0).point, ballPhysicBehaviour.LastVelocity.magnitude);

                BallEventManager.instance.OnBallCollision("Brick", other);

                ReturnInteration();
            }
            else
            {
                if (BallMultiplayerBehaviour.Instance.IsBallOwner)
                {
                    BallEventManager.instance.OnBallCollision("Brick", other);

                    AudioManager.instance.PlaySound("BrickExplosion", other.GetContact(0).point, ballPhysicBehaviour.LastVelocity.magnitude);

                    BallMultiplayerBehaviour.Instance.HandOverBallOwnership(BallOwnershipSwitchType.Return);
                }
                else
                {
                    ballPhysicBehaviour.FreezeBall();
                }
            }
        }
    }

    #region ReturnInteraction

    private void ReturnInteration()
    {
        StartCoroutine(RandomReturnWithoutBounce());
        SetMidWallStatus(false);
    }

    private IEnumerator RandomReturnWithoutBounce()
    {
        Vector3 targetPosition = targetSelector.GetNewTargetPosition();
        Vector3 newVelocity = nBMagicReturn.CalculateNewVelocity(transform.position, targetPosition);

        ballPhysicBehaviour.SetGravityState(false);
        ballPhysicBehaviour.PhysicRelatedVelocityUpdate(Vector3.zero);

        float timer = 0f;
        while (timer < bounceDelay)
        {
            if (!GameManager.Instance.IsGamePaused)
                timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ballPhysicBehaviour.SetGravityState(true);
        ballPhysicBehaviour.OverrideRawVelocity(newVelocity, (int)SpeedState.SLOW, true);
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
