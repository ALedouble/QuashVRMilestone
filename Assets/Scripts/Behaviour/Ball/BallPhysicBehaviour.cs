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
    public float startGlobalSpeedMultiplier = 1;
    private float globalSpeedMultiplier;
    public float GlobalSpeedMultiplier 
    {
        get => globalSpeedMultiplier;
        set
        {
            globalSpeedMultiplier = value;
            BallRigidbody.velocity *= value;

            UpdateCurrentGravity();
        } 
    }
    public float slowness;

    [Header("Gravity Settings")]
    public float baseGravity;
    public float CurrentGravity { get; private set; }
    public bool isSubjectToGravity { get; set; }

    [Header("Standard Bounce Settings")]
    public float bounciness;
    public float dynamicFriction;

    [Header("Multiplayer Settings")]
    public float followerCollisionActivationDelay = 0.2f;

    private PhotonView photonView;
    public Rigidbody BallRigidbody { get; set; }
    public Collider BallCollider { get; private set; }
    public Vector3 LastVelocity { get; set; }

    private SpeedState speedState;
    public SpeedState SpeedState 
    {
        get => speedState;
        private set
        {
            speedState = value;
            UpdateCurrentGravity();
        }
    }

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

        globalSpeedMultiplier = startGlobalSpeedMultiplier;

        ResetGravity();
    }

    private void Start()
    {
        BallMultiplayerBehaviour.Instance.OnBallOwnershipLoss += DelayedCollisionActivation;
    }

    private void FixedUpdate()
    {
        if(!BallManager.instance.IsBallPaused)
        {
            LastVelocity = BallRigidbody.velocity;

            ApplyForces();
        }
    }
    public void ResetBall()
    {
        SpeedState = SpeedState.NORMAL;
        BallRigidbody.velocity = Vector3.zero;
        CurrentGravity = 0;
    }

    #region Pause

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

    #endregion

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
        if(isSubjectToGravity)
            BallRigidbody.AddForce(CurrentGravity * Vector3.down);
    }

    public void SetGravityState(bool state)
    {
        isSubjectToGravity = state;
    }

    private void UpdateCurrentGravity()
    {
        if (SpeedState == SpeedState.NORMAL)
        {
            CurrentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier;
        }
        else if (SpeedState == SpeedState.SLOW)
        {
            CurrentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier / (slowness * slowness);
        }
    }

    public void ResetGravity()
    {
        CurrentGravity = baseGravity;
        isSubjectToGravity = true;
    }

    #endregion

    #region MovementHandling

    public void OverrideRawVelocity(Vector3 newVelocity, int newSpeedState, bool shouldUpdateVelocity)
    {
        BallRigidbody.velocity = newVelocity * globalSpeedMultiplier;
        LastVelocity = newVelocity * globalSpeedMultiplier;

        SpeedState = (SpeedState)newSpeedState;
        if(shouldUpdateVelocity)
            AdaptVelocityAndGravityToSpeedState();
    }

    public void PhysicRelatedVelocityUpdate(Vector3 newVelocity)
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

    private void AdaptVelocityAndGravityToSpeedState()
    {
        if(SpeedState == SpeedState.NORMAL)
        {
            BallRigidbody.velocity *= slowness;
            LastVelocity = BallRigidbody.velocity;
        }
        else if(SpeedState == SpeedState.SLOW)
        {
            BallRigidbody.velocity /= slowness;
            LastVelocity = BallRigidbody.velocity;
        }
    }

    #endregion

    #region Freeze

    public void FreezeBall()
    {
        velocityBeforeFreeze = BallRigidbody.velocity;
        gravityBeforeFreeze = CurrentGravity;

        BallRigidbody.velocity = Vector3.zero;
        CurrentGravity = 0;

        BallCollider.enabled = false;
    }

    public void UnFreezeBall()
    {
        BallRigidbody.velocity = velocityBeforeFreeze;
        CurrentGravity = gravityBeforeFreeze;

        BallCollider.enabled = true;
    }

    #endregion

    #region Ball First Spawn

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

        BallManager.instance.floatCoroutine = StartCoroutine(BallManager.instance.FloatCoroutine());
        BallCollider.enabled = true;
    }

    #endregion


    #region Collider

    public void DelayedCollisionActivation()
    {
        StartCoroutine(DelayedCollisionActivationCoroutine());
    }

    private IEnumerator DelayedCollisionActivationCoroutine()
    {
        yield return new WaitForSeconds(followerCollisionActivationDelay);

        BallCollider.enabled = true;
    }

    #endregion

    #region IPunObservable implementation
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(BallRigidbody.velocity);
            stream.SendNext((short)SpeedState);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            BallRigidbody.velocity = (Vector3)stream.ReceiveNext();
            SpeedState = (SpeedState)stream.ReceiveNext(); 
        }
    }
    #endregion
}
