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
    private BallStatus currentBallStatus = BallStatus.Inactive;
    public int WallHitCount { get => wallHitCount; }
    public BallStatus CurrentBallStatus { get => currentBallStatus; }
    public QPlayer LastPlayerWhoHitTheBall { get; set; }

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

    private void IncrementWallHitCount()
    {
        wallHitCount++;

        if(currentBallStatus == BallStatus.HitState && wallHitCount == 1)
        {
            BallManager.instance.SendOnFirstBounceEvent();
        }
    }

    private void ResetHitWallCount()
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

    private void EnterHitState()
    {
        currentBallStatus = BallStatus.HitState;
    }

    private void EnterReturnState()
    {
        currentBallStatus = BallStatus.ReturnState;

        BallManager.instance.SendOnReturnStart();
    }
}
