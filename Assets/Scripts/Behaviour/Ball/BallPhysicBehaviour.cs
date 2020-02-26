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

public enum RacketInteractionType
{
    BASICARCADE,
    BASICPHYSIC,
    MEDIUMPHYSIC,
    MIXED
}

public enum TargetSwitchType
{
    RACKETBASED,
    WALLBASED
}

public class BallPhysicBehaviour : MonoBehaviour, IPunObservable
{
    //Ajouter Le slow en RPC
    private enum SpeedState
    {
        NORMAL = 0,
        SLOW = 1
    }

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

    [Header("Racket Fake Physics Settings")]
    public float maxYAngle;
    public float maxZAngle;
    public float magnitudeSlope;
    public float minMagnitude;
    public float maxMagnitude;

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

    //public Transform[] xReturnsPoints = new Transform[];
    //public Transform zReturnPoints;

    [Header("Standard Bounce Settings")]
    public float bounciness;
    public float dynamicFriction;

    [Header("Sound Settings")]
    public float averageHitMagnitude = 10f;                            // A Changer!

    private PhotonView photonView;
    private Rigidbody rigidbody;
    public Collider BallCollider { get; private set; }

    private SpeedState speedState;
    private Vector3 velocityBeforeFreeze = Vector3.zero;
    private float gravityBeforeFreeze = 0;
    private Vector3 lastVelocity;

    private QPlayer lastPlayerWhoHitTheBall;

    private List<Vector3> forcesToApply;


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
    }

    private void FixedUpdate()
    {
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour les calculs de rebond
        
        ApplyForces();
    }

    private void OnCollisionEnter(Collision other)
    {

        switch (other.gameObject.tag)
        {
            case "Racket":
                RacketInteraction(other);
                VibrationManager.instance.VibrateOn("Vibration_Racket_Hit");
                AudioManager.instance.PlaySound("RacketHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude / averageHitMagnitude);
                break;
            case "FrontWall":
                ReturnInteration();
                AudioManager.instance.PlaySound("FrontWallHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude / averageHitMagnitude);
                break;
            case "Brick":
                ReturnInteration();
                AudioManager.instance.PlaySound("BrickExplosion", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude / averageHitMagnitude);
                break;
            case "BackWall":
                BallManager.instance.LoseBall();
                break;
            case "Floor":
                BallManager.instance.StartBallResetCountdown();
                StandardBounce(other.GetContact(0));
                AudioManager.instance.PlaySound("FloorHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude / averageHitMagnitude);
                break;
            case "Wall":
                StandardBounce(other.GetContact(0));
                AudioManager.instance.PlaySound("WallHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude / averageHitMagnitude);
                break;
            default:
                StandardBounce(other.GetContact(0));
                break;
        }

        SendBallCollisionEvent(other.gameObject.tag);

        //Revoir audio manager pour qu'il utilise le OnBallCollision event system?
        
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
            photonView.RPC("OnBallCollisionRPC", RpcTarget.All, tag);
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

    [PunRPC]
    private void ApplyNewVelocity(Vector3 newVelocity, Vector3 positionWhenHit, int newSpeedState, bool IsSpeedStateChangingSpeed)
    {
        transform.position = positionWhenHit;
        rigidbody.velocity = newVelocity;
        lastVelocity = newVelocity;
        SetSpeedState((SpeedState)newSpeedState, IsSpeedStateChangingSpeed);
    }

    private void ApplyNewVelocity(Vector3 newVelocity)
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
                currentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier;
            }

            speedState = SpeedState.NORMAL;
        }
        else if(newSpeedState == SpeedState.SLOW)
        {
            if(doesSpeedNeedToChange)
            {
                rigidbody.velocity /= slowness;
                lastVelocity = rigidbody.velocity;
                currentGravity = baseGravity * globalSpeedMultiplier * globalSpeedMultiplier / (slowness * slowness);
            }

            speedState = SpeedState.SLOW;
        }
    }

    public void SetGlobalSpeedMultiplier(float newValue)
    {
        globalSpeedMultiplier = newValue;
    }

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

    #region Standard Bounce

    /// Méthode qui calcul le rebond de la balle (calcul vectorielle basique) et modifie la trajectoire en conséquence
    /// contactPoint : données de collision entre la balle et l'autre objet
    private void StandardBounce(ContactPoint contactPoint)
    {
        Vector3 normal = Vector3.Normalize(contactPoint.normal);
        float normalVelocity = Vector3.Dot(normal, lastVelocity);
        if (normalVelocity > 0)
            normalVelocity = -normalVelocity;

        Vector3 tangent = Vector3.Normalize(lastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, lastVelocity);

        ApplyNewVelocity(((1 - dynamicFriction) * tangentVelocity * tangent - bounciness * normalVelocity * normal));
    }

    #endregion

    #region ReturnMechanics

    private void ReturnInteration()
    {
        //MagicalBounce3();
        RandomReturnWithoutBounce();
        //RandomReturnWithBounce();
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

    private Vector3 RacketFakeHit1(Collision collision)                     //Rotation avec bijection mal définie... Ce sera certainnement pas terrible...
    {
        Vector3 normal = collision.GetContact(0).normal;

        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, normal);
        float yEulerRotation = hitRotation.eulerAngles.y;
        float zEulerRotation = hitRotation.eulerAngles.z;   //Ce serait pas plutôt X qu'on veut?

        float newYEulerRotation = (yEulerRotation % 180) * (maxYAngle / 180);
        float newZEulerRotation = (zEulerRotation % 180) * (maxZAngle / 180);

        Vector3 velocityDirection = Vector3.Normalize(new Vector3(Mathf.Cos(newYEulerRotation), 1 / Mathf.Tan(newZEulerRotation), Mathf.Sin(newYEulerRotation)));

        float velocityMagnitude = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity().magnitude;
        velocityMagnitude *= magnitudeSlope;

        if (velocityMagnitude > maxMagnitude)
            velocityMagnitude = maxMagnitude;
        if (velocityMagnitude < minMagnitude)
            velocityMagnitude = minMagnitude;

        return velocityMagnitude * velocityDirection;
    }

    private Vector3 RacketFakeHit2(Collision collision)                     //Rotation avec bijection bien definie!                 Besoin d'un Tool?
    {
        // A Implementer
        return Vector3.zero;
    }

    private Vector3 RacketFakeHit3(Collision collision)                     //VitesseAngulaire probablement pas une bonne idée...
    {
        // A Implementer
        return Vector3.zero;
    }

    #endregion

    #region UilityMethods

    public void StartBallFirstSpawnCoroutine(float duration)
    {
        Debug.Log("StartBallColiderCoroutine");
        StartCoroutine(BallFirstSpawnCoroutine(duration));
    }

    private IEnumerator BallFirstSpawnCoroutine(float duration)
    {
        Debug.Log("Collider desactivé " + BallCollider);
        BallCollider.enabled = false;
        yield return new WaitForSeconds(duration);
        BallCollider.enabled = true;
        Debug.Log("Collider réactivé " + BallCollider);
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
