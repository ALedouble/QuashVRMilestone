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
    //Ajouter Le slow en RPC
    

    [Header("Racket Settings")]
    public RacketInteractionType physicsUsed;

    // Animation curve
    public float hitMaxSpeed;
    public float hitMinSpeed;
    public float hitSpeedMultiplier;

    [Header("Racket Physics Settings")]
    public float racketFriction;

    [Header("Racket Mixed Physics Settings")]
    [Range(0, 1)]
    public float mixRatio;

    [Header("Speed Settings")]
    public float globalSpeedMultiplier = 1;
    public float slowness;

    [Header("Gravity Settings")]
    public float baseGravity;
    private float currentGravity;

    [Header("MagicBounce Settings")]
    public float depthVelocity;

    public float xAcceleration;
    public float groundHeight;

    public float minRange;
    public float maxRange;
    public float angleSpread;

    private ITargetSelector targetSelector;
    private OneBounceMagicReturn oBMagicReturn;
    private NoBounceMagicReturn nBMagicReturn;

    [Header("Switch Target Settings")]
    public TargetSwitchType switchType = TargetSwitchType.RACKETBASED;
    public QPlayer startingPlayer = QPlayer.PLAYER1;                  // Sera modifier!
    private bool switchTargetIsRacketBased;

    [Header("Standard Bounce Settings")]
    public float bounciness;
    public float dynamicFriction;

    [Header("Sound Settings")]
    public float averageHitMagnitude = 10f;                            // A Changer!

    private PhotonView photonView;
    [HideInInspector] public Rigidbody rigidbody;
    public Collider BallCollider { get; private set; }

    private SpeedState speedState;
    private Vector3 velocityBeforeFreeze = Vector3.zero;
    private float gravityBeforeFreeze = 0;
    private Vector3 lastVelocity;

    private QPlayer lastPlayerWhoHitTheBall;

    private List<Vector3> forcesToApply;
    private Vector3 pauseSavedVelocity;

    private Coroutine IgnoreCollisionCoroutine;

    private bool isOnFrontWallCollisionFrame;
    private bool IsOnFrontWallCollisionFrame { 
        get => isOnFrontWallCollisionFrame;
        set 
        {
            if (value == true)
                StartCoroutine(ResetFrontWallCollisionBoolValue());

            isOnFrontWallCollisionFrame = value;
        } 
    }
    

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();
        BallCollider = GetComponent<Collider>();

        targetSelector = GetComponent<BasicRandomTargetSelector>();
        targetSelector.SetCurrentTarget(startingPlayer);

        oBMagicReturn = new OneBounceMagicReturn(depthVelocity, xAcceleration, baseGravity, bounciness, dynamicFriction, groundHeight);
        nBMagicReturn = new NoBounceMagicReturn(depthVelocity, baseGravity, xAcceleration);

        forcesToApply = new List<Vector3>();

        InitialiseTargetSwitchType();

        ApplyBaseGravity();

        IsOnFrontWallCollisionFrame = false;
    }

    private void FixedUpdate()
    {
        if(!BallManager.instance.IsBallPaused)
        {
            lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour les calculs de rebond

            ApplyForces();
        }
    }

    public void PauseBallPhysics()
    {
        pauseSavedVelocity = rigidbody.velocity;
        rigidbody.velocity = Vector3.zero;
    }

    public void ResumeBallPhysics()
    {
        rigidbody.velocity = pauseSavedVelocity;
        pauseSavedVelocity = Vector3.zero;
    }

    #region Collision

    private void OnCollisionEnter(Collision other)
    {

    }

    private void OnCollisionExit(Collision collision)
    {
        SendBallCollisionExitEvent(collision.gameObject.tag);
    }

    private void SendBallCollisionEvent(string tag)
    {
        if (GameManager.Instance.offlineMode)
        {
            OnBallCollisionRPC(tag);
        }
        else if (tag == "Racket")
        {
            //photonView.RPC("OnBallCollisionRPC", RpcTarget.All, tag);
            BallEventManager.instance.OnBallCollision(tag);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("OnBallCollisionRPC", RpcTarget.All, tag);
            }
        }
    }

    [PunRPC]
    private void OnBallCollisionRPC(string tag)
    {
        BallEventManager.instance.OnBallCollision(tag);
    }

    private void SendBallCollisionExitEvent(string tag)
    {
        if (GameManager.Instance.offlineMode)
        {
            OnBallCollisionExitRPC(tag);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("OnBallCollisionExitRPC", RpcTarget.All, tag);
        }
    }

    [PunRPC]
    private void OnBallCollisionExitRPC(string tag)
    {
        BallEventManager.instance.OnBallCollisionExit(tag);
    }

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

    private IEnumerator ResetFrontWallCollisionBoolValue()
    {
        if (!BallManager.instance.IsBallPaused)
        {
            yield return new WaitForFixedUpdate();
            isOnFrontWallCollisionFrame = false;
        }
    }
    #endregion

    #region MovementHandling

    [PunRPC]
    public void ApplyNewVelocity(Vector3 newVelocity, Vector3 positionWhenHit, int newSpeedState, bool IsSpeedStateChangingSpeed)
    {
        transform.position = positionWhenHit;
        rigidbody.velocity = newVelocity;
        lastVelocity = newVelocity;
        SetSpeedState((SpeedState)newSpeedState, IsSpeedStateChangingSpeed);
    }

    public void ApplyNewVelocity(Vector3 newVelocity)
    {
        rigidbody.velocity = newVelocity;
    }

    private void ApplyForces()
    {
        ApplyGravity();

        if (forcesToApply.Count > 0)
        {
            foreach (Vector3 force in forcesToApply)
            {
                rigidbody.AddForce(force);
            }
        }
    }

    private void ApplyGravity()
    {
        rigidbody.AddForce(currentGravity * Vector3.down);
    }

    private void FreezeBall()
    {
        velocityBeforeFreeze = rigidbody.velocity;
        gravityBeforeFreeze = currentGravity;

        rigidbody.velocity = Vector3.zero;
        currentGravity = 0;

        BallManager.instance.BallPhysicInfo.SaveCurrentState();
        BallCollider.enabled = false;
    }

    private void UnFreezeBall()
    {
        rigidbody.velocity = velocityBeforeFreeze;
        currentGravity = gravityBeforeFreeze;

        BallManager.instance.BallPhysicInfo.RestoreSavedState();
        BallCollider.enabled = true;
    }

    private void SetSpeedState(SpeedState newSpeedState, bool doesSpeedNeedToChange)
    {
        if(newSpeedState == SpeedState.NORMAL)
        {
            if(doesSpeedNeedToChange)
            {
                rigidbody.velocity *= slowness;
                lastVelocity = rigidbody.velocity;
            }

            speedState = SpeedState.NORMAL;
        }
        else if(newSpeedState == SpeedState.SLOW)
        {
            if(doesSpeedNeedToChange)
            {
                rigidbody.velocity /= slowness;
                lastVelocity = rigidbody.velocity;
            }

            speedState = SpeedState.SLOW;
        }

        UpdateCurrentGravity();
    }

    public void SetGlobalSpeedMultiplier(float newValue)
    {
        globalSpeedMultiplier = newValue;
        rigidbody.velocity *= newValue;

        UpdateCurrentGravity();
    }

    private void UpdateCurrentGravity()
    {
        if (speedState == SpeedState.NORMAL)
        {
            currentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier;
        }
        else if (speedState == SpeedState.SLOW)
        {
            currentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier / (slowness * slowness);
        }
    }

    #endregion

    #region RacketInteraction

    private void RacketInteraction(Collision other)
    {
        ApplyRacketPhysic(other);

        RacketManager.instance.OnHitEvent(gameObject);  // Ignore collision pour quelques frames.

        SetLastPlayerWhoHitTheBall();
        RacketBasedSwitchTarget();
        SetMidWallStatus(true);
    }

    private void ApplyRacketPhysic(Collision other)
    {
        Vector3 newVelocity = Vector3.zero;

        switch (physicsUsed)
        {
            case RacketInteractionType.BASICARCADE:
                newVelocity = RacketArcadeHit();
                break;

            case RacketInteractionType.BASICPHYSIC:
                newVelocity = RacketBasicPhysicHit(other);
                break;

            case RacketInteractionType.MEDIUMPHYSIC:
                newVelocity = RacketMediumPhysicHit(other);
                break;

            case RacketInteractionType.MIXED:
                newVelocity = RacketMixedHit(other);
                break;
        }

        newVelocity = ClampVelocity(hitSpeedMultiplier * newVelocity);

        if (GameManager.Instance.offlineMode)
        {
            ApplyNewVelocity(newVelocity * globalSpeedMultiplier, transform.position, (int)SpeedState.NORMAL, false);                                  // Modif globalSpeedMultiplier
        }
        else
        {
            photonView.RPC("ApplyNewVelocity", RpcTarget.All, newVelocity * globalSpeedMultiplier, transform.position, (int)SpeedState.NORMAL, false);     // Modif globalSpeedMultiplier
        }
    }
    
    private void SetLastPlayerWhoHitTheBall()
    {
        if(GameManager.Instance.offlineMode)
        {
            lastPlayerWhoHitTheBall = QPlayer.PLAYER1;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SetLastPlayerToHitTheBallToPlayer1", RpcTarget.All);
            }
            else
            {
                photonView.RPC("SetLastPlayerToHitTheBallToPlayer2", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void SetLastPlayerToHitTheBallToPlayer1()
    {
        lastPlayerWhoHitTheBall = QPlayer.PLAYER1;
    }

    [PunRPC]
    private void SetLastPlayerToHitTheBallToPlayer2()
    {
        lastPlayerWhoHitTheBall = QPlayer.PLAYER2;
    }

    private void RacketBasedSwitchTarget()
    {
        if (switchTargetIsRacketBased)
        {
            if (GameManager.Instance.offlineMode)                                              // Really?
            {
                SwitchTarget();
            }
            else
            {
                photonView.RPC("SwitchTarget", RpcTarget.All);
            }
        }
    }

    #endregion

    #region BallReturn

    private void ReturnInteration()
    {
        //MagicalBounce3();
        RandomReturnWithoutBounce();
        //RandomReturnWithBounce();
        IsOnFrontWallCollisionFrame = true;
        SetMidWallStatus(false);
    }

    #region BasicMagicReturn
    private void MagicalBounce3()           //Question: Repère par raport au terrain
    {
        if (!switchTargetIsRacketBased && photonView.IsMine)
        {
            photonView.RPC("SwitchTarget", RpcTarget.All);
        }

        //RPC Slow?

        float verticalVelocity = CalculateVerticalBounceVelocity();
        float sideVelocity = CalculateSideBounceVelocity();

        ApplyNewVelocity(new Vector3(sideVelocity, verticalVelocity, -depthVelocity), transform.position, (int)SpeedState.SLOW, true);
    }

    private float CalculateVerticalBounceVelocity()
    {
        return (baseGravity * (targetSelector.GetTargetPlayerPosition().z - transform.position.z) / -depthVelocity / 2) - (transform.position.y * -depthVelocity / (targetSelector.GetTargetPlayerPosition().z - transform.position.z));
    }

    private float CalculateSideBounceVelocity()
    {
        Vector3 returnHorizontalDirection = new Vector3(targetSelector.GetTargetPlayerPosition().x - transform.position.x, 0, targetSelector.GetTargetPlayerPosition().z - transform.position.z);
        returnHorizontalDirection = Vector3.Normalize(returnHorizontalDirection);
        return (Vector3.Dot(Vector3.right, returnHorizontalDirection) / Vector3.Dot(Vector3.back, returnHorizontalDirection)) * depthVelocity;
    }

    [PunRPC]
    private void SwitchTarget()
    {
        targetSelector.SwitchTarget();
    }
    #endregion

    #region RandomReturn

    private void RandomReturnWithBounce()
    {
        Vector3 targetPosition = targetSelector.GetNewTargetPosition();
        Vector3 newVelocity = oBMagicReturn.CalculateNewVelocity(transform.position, targetPosition);

        ApplyNewVelocity(newVelocity, transform.position, (int)SpeedState.SLOW, true);
    }

    private void RandomReturnWithoutBounce()
    {
        Vector3 targetPosition = targetSelector.GetNewTargetPosition();
        Vector3 newVelocity = nBMagicReturn.CalculateNewVelocity(transform.position, targetPosition);

        ApplyNewVelocity(newVelocity * globalSpeedMultiplier, transform.position, (int)SpeedState.SLOW, true);
    }

    #endregion

    #endregion

    #region RacketInteraction

    private Vector3 RacketArcadeHit()
    {
        return RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity();
    }

    private Vector3 RacketBasicPhysicHit(Collision collision)       // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        Vector3 relativeVelocity = lastVelocity - racketVelocity;
        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;
        Vector3 tangentVelocity = (relativeVelocity - normalVelocity) * (1 - racketFriction);        // Ajouter frottement

        return -normalVelocity + tangentVelocity;
    }

    private Vector3 RacketMediumPhysicHit(Collision collision) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = (2 * Vector3.Dot(contactPointNormal, racketVelocity) - Vector3.Dot(contactPointNormal, lastVelocity)) * contactPointNormal;
        Vector3 tangentVelocity = (lastVelocity - Vector3.Dot(contactPointNormal, lastVelocity) * contactPointNormal) * (1 - racketFriction);        // Ajouter frottement

        return normalVelocity + tangentVelocity;
    }

    private Vector3 RacketMixedHit(Collision collision)
    {
        return RacketArcadeHit() * (1 - mixRatio) + RacketBasicPhysicHit(collision) * mixRatio;
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
        rigidbody.velocity = Vector3.zero;
        currentGravity = 0;
    }

    public void ApplyBaseGravity()
    {
        currentGravity = baseGravity;
    }

    private void InitialiseTargetSwitchType()
    {
        if (switchType == TargetSwitchType.RACKETBASED)
        {
            switchTargetIsRacketBased = true;
        }
        else
        {
            switchTargetIsRacketBased = false;
        }
    }

    private Vector3 ClampVelocity(Vector3 velocity)        //Nom à modifier
    {
        if (velocity.magnitude < hitMinSpeed)
        {
            return hitMinSpeed * Vector3.Normalize(velocity);
        }
        else if (velocity.magnitude > hitMaxSpeed)
        {
            return hitMaxSpeed * Vector3.Normalize(velocity);
        }
        else
            return velocity;
    }

    private float MakeLinearAssociation(float variable, float slope, float offset)
    {
        return slope * variable + offset;
    }

    public QPlayer GetLastPlayerWhoHitTheBall()
    {
        return lastPlayerWhoHitTheBall;
    }

    public ITargetSelector GetTargetSelector()
    {
        return targetSelector;
    }

    #endregion

    #region MidWallInterraction

    private void SetMidWallStatus(bool isCollidable)
    {
        if(GameManager.Instance.offlineMode)
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
        if(LevelManager.instance.numberOfPlayers > 1)
        {
            //LevelManager.instance.midCollider.enabled = false;
            LevelManager.instance.midCollider.gameObject.SetActive(false);
        }
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
