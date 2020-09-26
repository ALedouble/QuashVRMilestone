using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallStatus
{
    Inactive,
    FloatingState,
    HitState,
    ReturnState
}

public class BallInfo : MonoBehaviour
{
    private int wallHitCount;
    public int WallHitCount { get => wallHitCount; }
    private BallStatus currentBallStatus = BallStatus.Inactive;
    public BallStatus CurrentBallStatus { get => currentBallStatus; }
    private QPlayer lastPlayerWhoHitTheBall;
    public QPlayer LastPlayerWhoHitTheBall
    {
        get => lastPlayerWhoHitTheBall;
        set
        {
            lastPlayerWhoHitTheBall = value;

            if(!GameManager.Instance.offlineMode && BallMultiplayerBehaviour.Instance.IsBallOwner)
            {
                photonView.RPC("SetLastPlayerWhoHitTheBall", RpcTarget.Others, value);
            }
        }
    }
    
    private PhotonView photonView;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SetupBallInfo()
    {
        BallEventManager.instance.OnCollisionWithWall += IncrementWallHitCount;
        BallEventManager.instance.OnCollisionWithFloor += IncrementWallHitCount;

        BallEventManager.instance.OnCollisionWithRacket += ResetHitWallCount;
        BallEventManager.instance.OnCollisionWithRacket += EnterHitState;

        BallEventManager.instance.OnCollisionWithFrontWall += EnterReturnState;
        BallEventManager.instance.OnCollisionWithRebounceWall += EnterReturnState;
        BallEventManager.instance.OnCollisionWithBrick += EnterReturnState;

        BallEventManager.instance.OnBallSpawn += EnterFloatingState;
        BallEventManager.instance.OnBallDespawn += EnterInactiveState;
    }

    private void IncrementWallHitCount(Collision collision)
    {
        wallHitCount++;

        if(currentBallStatus == BallStatus.HitState && wallHitCount == 1)
        {
            BallManager.instance.SendOnFirstBounceEvent();
        }
    }

    private void ResetHitWallCount(Collision collision)
    {
        wallHitCount = 0;
    }

    public void EnterInactiveState()
    {
        currentBallStatus = BallStatus.Inactive;
    }

    private void EnterFloatingState()
    {
        currentBallStatus = BallStatus.FloatingState;
    }

    private void EnterHitState(Collision collision)
    {
        currentBallStatus = BallStatus.HitState;
    }

    private void EnterReturnState(Collision collision)
    {
        currentBallStatus = BallStatus.ReturnState;

        BallManager.instance.SendOnReturnStart();
    }

    #region Multiplayer Setter

    [PunRPC]
    private void SetLastPlayerWhoHitTheBall(QPlayer playerID)
    {
        LastPlayerWhoHitTheBall = playerID;
    }

    #endregion
}
