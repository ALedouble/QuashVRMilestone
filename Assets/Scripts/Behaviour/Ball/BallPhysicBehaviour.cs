using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public enum Target          //Util?
{
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
        NORMAL,
        SLOW
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

    [Header("Slow Return Settings")]
    public float slowness;

    [Header("Gravity Settings")]
    public float gravity;

    [Header("MagicBounce Settings")]
    public Target startingPlayer = Target.PLAYER1;                  // Sera modifier!
    private Target currentTarget;
    public float depthVelocity;

    [Header("Switch Target Settings")]
    public TargetSwitchType switchType = TargetSwitchType.RACKETBASED;
    private bool switchTargetIsRacketBased;

    public Transform[] xReturnsPoints = new Transform[8];
    public Transform zReturnPoints;

    [Header("Standard Bounce Settings")]
    public float bounciness;
    public float dynamicFriction;


    private Rigidbody rigidbody;
    private SpeedState speedState;
    private Vector3 lastVelocity;

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();

        speedState = SpeedState.NORMAL;

        currentTarget = startingPlayer;

        InitialiseTargetSwitchType();
    }

    private void FixedUpdate()
    {
        if(PhotonNetwork.OfflineMode){
            Movement();
        } else if (!PhotonNetwork.OfflineMode){
            photonView.RPC("Movement", RpcTarget.All);
        }
        
    }

    [PunRPC]
    private void Movement(){
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)
        if (speedState == SpeedState.NORMAL)
            rigidbody.AddForce(gravity * Vector3.down);
        else if (speedState == SpeedState.SLOW)
            rigidbody.AddForce(gravity / (slowness * slowness) * Vector3.down);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Racket":
                RacketInteraction(other);
                speedState = SpeedState.NORMAL;                                         //La?
                break;
            case "FrontWall":
            case "Brick":
                MagicalBounce3();
                speedState = SpeedState.SLOW;           // Pourquoi pas dans la méthode?
                break;
            default:
                StandardBounce(other.GetContact(0));
                break;
        }


        //photonView.RPC("OnBallCollision", RpcTarget.All, other.gameObject.tag);

        if (PhotonNetwork.OfflineMode)
        {
            //OnBallCollision(new BallCollisionInfo(other.gameObject.tag, other.GetContact(0).point, other.GetContact(0).normal, lastVelocity));
            OnBallCollision(other.gameObject.tag);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("OnBallCollision", RpcTarget.All, other.gameObject.tag);
        }

        //Revoir audio manager pour qu'il utilise le OnBallCollision event system
        AudioManager.instance?.PlayHitSound(other.gameObject.tag, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal), RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity().magnitude);
    }

    //[PunRPC]
    //private void OnBallCollision(BallCollisionInfo ballCollisionInfo)
    //{
    //    //BallEventManager.instance?.OnBallCollision(other.gameObject.tag);
    //    BallEventManager.instance?.OnBallCollision(ballCollisionInfo);
    //}

    [PunRPC]
    private void OnBallCollision(string tag)
    {
        //BallEventManager.instance?.OnBallCollision(other.gameObject.tag);
        BallEventManager.instance.OnBallCollision(tag);
    }


    #region RacketInteraction

    private void RacketInteraction(Collision other)
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

        photonView.RPC("RacketApplyNewVelocity", RpcTarget.All, newVelocity, transform.position);
        RacketManager.instance.OnHitEvent(gameObject);  // Ignore collision pour quelques frames.

        if (switchTargetIsRacketBased)
        {
            photonView.RPC("SwitchTarget", RpcTarget.All);
        }

        BallManager.instance.TransferEmpowerement();

        //if (PhotonNetwork.OfflineMode)
        //{
        //    RacketApplyNewVelocity(newVelocity, transform.position);
        //    RacketManager.instance.OnHitEvent(gameObject);  // Ignore collision pour quelques frames.
        //}
        //else //if(other.gameObject.GetComponent<PhotonView>().IsMine)
        //{
        //    photonView.RPC("RacketApplyNewVelocity", RpcTarget.All, newVelocity, transform.position);
        //    RacketManager.instance.OnHitEvent(gameObject);  // Ignore collision pour quelques frames.

        //    if (switchIsRacketBased)
        //    {
        //        photonView.RPC("SwitchTarget", RpcTarget.All);
        //    }
        //}
    }

    [PunRPC]
    private void RacketApplyNewVelocity(Vector3 newVelocity, Vector3 positionWhenHit)
    {
        transform.position = positionWhenHit;
        rigidbody.velocity = newVelocity;
    }

    #endregion

    #region Standard Bounce

    /// Méthode qui calcul le rebond de la balle (calcul vectorielle basique) et modifie la trajectoire en conséquence
    /// contactPoint : données de collision entre la balle et l'autre objet

    private void StandardBounce(ContactPoint contactPoint)
    {
        Vector3 normal = Vector3.Normalize(contactPoint.normal);
        float normalVelocity = Vector3.Dot(normal, lastVelocity);

        Vector3 tangent = Vector3.Normalize(lastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, lastVelocity);

        rigidbody.velocity = ((1 - dynamicFriction) * tangentVelocity * tangent - bounciness * normalVelocity * normal);
    }

    #endregion

    #region ReturnMechanics

    private void MagicalBounce3()           //Question: Repère par raport au terrain
    {
        if (!switchTargetIsRacketBased && photonView.IsMine)
        {
            photonView.RPC("SwitchTarget", RpcTarget.All);
        }

        //RPC Slow?

        float verticalVelocity = CalculateVerticalBounceVelocity();
        float sideVelocity = CalculateSideBounceVelocity();

        rigidbody.velocity = new Vector3(sideVelocity, verticalVelocity, -depthVelocity) / slowness;
    }

    private float CalculateVerticalBounceVelocity()
    {
        return (gravity * (GetCurrentTargetPositionZ().z - transform.position.z) / -depthVelocity / 2) - (transform.position.y * -depthVelocity / (GetCurrentTargetPositionZ().z - transform.position.z));
    }

    private float CalculateSideBounceVelocity()
    {
        Vector3 returnHorizontalDirection = new Vector3(GetCurrentTargetPositionX().x - transform.position.x, 0, GetCurrentTargetPositionX().z - transform.position.z);
        returnHorizontalDirection = Vector3.Normalize(returnHorizontalDirection);
        return (Vector3.Dot(Vector3.right, returnHorizontalDirection) / Vector3.Dot(Vector3.back, returnHorizontalDirection)) * depthVelocity;
    }

    [PunRPC]
    private void SwitchTarget()
    {
        if (currentTarget == Target.PLAYER1)
            currentTarget = Target.PLAYER2;
        else if (currentTarget == Target.PLAYER2)
            currentTarget = Target.PLAYER1;
        //currentTarget = (Target)(((int)currentTarget + 1) % PhotonNetwork.PlayerList.Length);
    }

    private Vector3 GetCurrentTargetPositionX()
    {
        if (currentTarget == Target.PLAYER1)
        {
            return xReturnsPoints[0].position;
        }
        else
        {
            return xReturnsPoints[1].position;
        }
    }

    private Vector3 GetCurrentTargetPositionZ()
    {
        return zReturnPoints.position;
    }

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
