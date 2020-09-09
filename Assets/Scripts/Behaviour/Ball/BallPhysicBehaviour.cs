using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public enum QPlayer          //Util?
{
    NONE = -1,
    PLAYER1 = 0,
    PLAYER2 = 1
}

public enum TargetSwitchType
{
    RACKETBASED,
    WALLBASED
}

public enum SpeedState
{
    NORMAL = 0,
    SLOW = 1
}

public class BallPhysicBehaviour : MonoBehaviour, IPunObservable
{
    [Header("Speed Settings")]
    public float globalSpeedMultiplier = 1;
    public float slowness;

    [Header("Gravity Settings")]
    public float baseGravity;
    public float CurrentGravity;

    [Header("Standard Bounce Settings")]
    public float bounciness;
    public float dynamicFriction;

    private PhotonView photonView;
    public Rigidbody BallRigidbody { get; set; }
    public Collider BallCollider { get; private set; }
    public Vector3 LastVelocity { get; set; }

    private SpeedState speedState;

    private Vector3 velocityBeforeFreeze = Vector3.zero;
    private float gravityBeforeFreeze = 0;

    private List<Vector3> forcesToApply;
    private Vector3 pauseSavedVelocity;
    

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        BallRigidbody = GetComponent<Rigidbody>();
        BallCollider = GetComponent<Collider>();

        forcesToApply = new List<Vector3>();

        ResetGravity();
    }

    private void FixedUpdate()
    {
        if(!BallManager.instance.IsBallPaused)
        {
            LastVelocity = BallRigidbody.velocity;

            ApplyForces();
        }
    }

    public void PauseBallPhysics()
    {
        pauseSavedVelocity = BallRigidbody.velocity;
        BallRigidbody.velocity = Vector3.zero;
    }

    public void ResumeBallPhysics()
    {
        BallRigidbody.velocity = pauseSavedVelocity;
        pauseSavedVelocity = Vector3.zero;
    }

    #region Collision

    private IEnumerator IgnoreCollision()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), true);

        float timer = 0f;
        while(timer < 0.1f)
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

    #region Gravity

    private void ApplyGravity()
    {
        BallRigidbody.AddForce(CurrentGravity * Vector3.down);
    }

    private void UpdateCurrentGravity()
    {
        if (speedState == SpeedState.NORMAL)
        {
            CurrentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier;
        }
        else if (speedState == SpeedState.SLOW)
        {
            CurrentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier / (slowness * slowness);
        }
    }

    public void ResetGravity()
    {
        CurrentGravity = baseGravity;
    }

    #endregion

    #region MovementHandling

    [PunRPC]
    public void ApplyNewVelocity(Vector3 newVelocity, Vector3 positionWhenHit, int newSpeedState, bool IsSpeedStateChangingSpeed)
    {
        transform.position = positionWhenHit;
        BallRigidbody.velocity = newVelocity;
        LastVelocity = newVelocity;
        SetSpeedState((SpeedState)newSpeedState, IsSpeedStateChangingSpeed);
    }

    public void ApplyNewVelocity(Vector3 newVelocity)
    {
        BallRigidbody.velocity = newVelocity;
    }

    private void ApplyForces()
    {
        ApplyGravity();

        if (forcesToApply.Count > 0)
        {
            foreach (Vector3 force in forcesToApply)
            {
                BallRigidbody.AddForce(force);
            }
        }
    }

    private void FreezeBall()
    {
        velocityBeforeFreeze = BallRigidbody.velocity;
        gravityBeforeFreeze = CurrentGravity;

        BallRigidbody.velocity = Vector3.zero;
        CurrentGravity = 0;

        BallManager.instance.BallPhysicInfo.SaveCurrentState();
        BallCollider.enabled = false;
    }

    private void UnFreezeBall()
    {
        BallRigidbody.velocity = velocityBeforeFreeze;
        CurrentGravity = gravityBeforeFreeze;

        BallManager.instance.BallPhysicInfo.RestoreSavedState();
        BallCollider.enabled = true;
    }

    private void SetSpeedState(SpeedState newSpeedState, bool doesSpeedNeedToChange)
    {
        if(newSpeedState == SpeedState.NORMAL)
        {
            if(doesSpeedNeedToChange)
            {
                BallRigidbody.velocity *= slowness;
                LastVelocity = BallRigidbody.velocity;
            }

            speedState = SpeedState.NORMAL;
        }
        else if(newSpeedState == SpeedState.SLOW)
        {
            if(doesSpeedNeedToChange)
            {
                BallRigidbody.velocity /= slowness;
                LastVelocity = BallRigidbody.velocity;
            }

            speedState = SpeedState.SLOW;
        }

        UpdateCurrentGravity();
    }

    public void SetGlobalSpeedMultiplier(float newValue)
    {
        globalSpeedMultiplier = newValue;
        BallRigidbody.velocity *= newValue;

        UpdateCurrentGravity();
    }

    #endregion

    #region UilityMethods

    public void StartBallFirstSpawnCoroutine(float duration)
    {
        StartCoroutine(BallFirstSpawnCoroutine(duration));
    }

    private IEnumerator BallFirstSpawnCoroutine(float duration)
    {
        BallCollider.enabled = false;

        float timer = 0f;
        while(timer < duration)
        {
            yield return new WaitForFixedUpdate();
            if(!BallManager.instance.IsBallPaused)
            {
                timer += Time.fixedDeltaTime;
            }
        }

        BallCollider.enabled = true;
    }

    public void ResetBall()
    {
        speedState = SpeedState.NORMAL;
        BallRigidbody.velocity = Vector3.zero;
        CurrentGravity = 0;
    }

    private float MakeLinearAssociation(float variable, float slope, float offset)
    {
        return slope * variable + offset;
    }

    #endregion

    #region IPunObservable implementation
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
    #endregion
}
