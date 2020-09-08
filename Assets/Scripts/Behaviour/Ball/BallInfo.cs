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

    private Vector3 lastVelocity;
    public Vector3 LastVelocity { get => lastVelocity; }
    
    private bool isOnFrontWallCollisionFrame;
    public bool IsOnFrontWallCollisionFrame
    {
        get => isOnFrontWallCollisionFrame;
        private set
        {
            if (value == true)
                StartCoroutine(ResetFrontWallCollisionBoolValue());

            isOnFrontWallCollisionFrame = value;
        }
    }

    private Rigidbody ballRigidbody;

    private void Awake()
    {
        ballRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(!BallManager.instance.IsBallPaused)
            lastVelocity = ballRigidbody.velocity;  // Vitesse avant contact necessaire pour les calculs de rebond
    }

    public void SetupBallInfo()
    {
        BallEventManager.instance.OnCollisionWithWall += IncrementWallHitCount;
        BallEventManager.instance.OnCollisionWithFloor += IncrementWallHitCount;

        BallEventManager.instance.OnCollisionWithRacket += ResetHitWallCount;
        BallEventManager.instance.OnCollisionWithRacket += EnterHitState;

        BallEventManager.instance.OnCollisionWithFrontWall += EnterReturnState;
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
        IsOnFrontWallCollisionFrame = true;
        currentBallStatus = BallStatus.ReturnState;

        BallManager.instance.SendOnReturnStart();
    }

    #region FrontWallCollision

    private IEnumerator ResetFrontWallCollisionBoolValue()
    {
        do
        {
            yield return new WaitForFixedUpdate();
            isOnFrontWallCollisionFrame = false;
        }
        while (BallManager.instance.IsBallPaused);
    }

    #endregion
}
