using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class BallBehaviour : MonoBehaviourPunCallbacks/*, IPunObservable*/
{
    private enum BallState
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
    public int numberOfPlayer = 1;                                  // A placer ailleur!
    public Target startingPlayer = Target.PLAYER1;                  // Sera modifier!
    private Target currentTarget;
    public float depthVelocity;

    [Header("Switch Target Settings")]
    public TargetSwitchType switchType = TargetSwitchType.RACKETBASED;
    private bool switchIsRacketBased;

    public Transform[] playerTransforms = new Transform[8];
    public Transform[] xReturnsPoints = new Transform[8];
    public Transform zReturnPoints;

    [Header("Standard Bounce Settings")]
    public float bounciness;
    public float dynamicFriction;


    [Header("Color Settings")]
    public Material[] materials;
    private int colorID = 0;


    private Rigidbody rigidbody;
    Renderer renderer; 
    private BallState ballState;
    private Vector3 lastVelocity;

    PhotonView view;
    Vector3 myVel;
    

    void Start()
    {
        view = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();
        renderer = gameObject.GetComponent<Renderer>();

        ballState = BallState.NORMAL;
        currentTarget = startingPlayer;

        InitialiseTargetSwitchType();
    }

    private void FixedUpdate()
    {  
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)
        if (ballState == BallState.NORMAL)
            rigidbody.AddForce(gravity * Vector3.down);
        else if (ballState == BallState.SLOW)
            rigidbody.AddForce(gravity / (slowness * slowness) * Vector3.down);
    }

    private void OnCollisionEnter(Collision other)
    {
        AudioManager.instance?.PlayHitSound(other.gameObject.tag, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal), RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity().magnitude);
        
        if (other.gameObject.CompareTag("Racket"))
        {
            RacketInteraction(other);
        }
        else if (other.gameObject.CompareTag("FrontWall") || other.gameObject.CompareTag("Brick"))
        {
            Debug.Log("MagicBounce");
            MagicalBounce3(other);
            ballState = BallState.SLOW;
        }
        else
        {
            StandardBounce(other.GetContact(0));        // Util?
        }

        BallEventManager.instance?.OnBallCollision(new BallCollisionInfo(other.gameObject.tag, other.GetContact(0).point, other.GetContact(0).normal,lastVelocity));
        //BallEventManager.instance?.OnBallCollision(other.gameObject.tag);
    }

    /////////////////////////////////////////////    Racket Interaction Interaction     /////////////////////////////////////////////////
    
    private void RacketInteraction(Collision other)
    {
        Vector3 newVelocity = Vector3.zero;

        switch (physicsUsed)
        {
            case RacketInteractionType.BASICARCADE:

                newVelocity = RacketArcadeHit();
                myVel = newVelocity;
                break;

            case RacketInteractionType.BASICPHYSIC:

                newVelocity = RacketBasicPhysicHit(other);
                myVel = newVelocity;
                break;

            case RacketInteractionType.MEDIUMPHYSIC:

                newVelocity = RacketMediumPhysicHit(other);
                myVel = newVelocity;

                break;
            case RacketInteractionType.MIXED:

                newVelocity = RacketMixedHit(other);
                myVel = newVelocity;
                break;
        }

        OnHitCollision(newVelocity);
        RacketManager.instance.OnHitEvent(gameObject);  // Ignore collision pour quelques frames.

        if (numberOfPlayer > 1)                //Amelioration Check sur manager
        {
            view.RPC("OnHitCollision", RpcTarget.Others, myVel);

            if (switchIsRacketBased)
            {
                NetworkSwitchTarget();
            }
        }
    }

    [PunRPC]
    private void OnHitCollision(Vector3 direction)
    {
        myVel = direction;
        rigidbody.velocity = ClampVelocity(hitSpeedMultiplier * direction);
        ballState = BallState.NORMAL;
    }

    //////////////////////////////////    Wall-Floor Interaction     /////////////////////////////////////////////////

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

    private void MagicalBounce3(Collision collision)
    {
        if(!switchIsRacketBased)
        {
            NetworkSwitchTarget();
        }
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
        Vector3 returnHorizontalDirection = new Vector3(GetCurrentTargetPosition().x - transform.position.x, 0, GetCurrentTargetPosition().z - transform.position.z);
        returnHorizontalDirection = Vector3.Normalize(returnHorizontalDirection);
        //return Vector3.Dot(depthVelocity * Vector3.back, returnHorizontalDirection) * Vector3.Dot(returnHorizontalDirection, Vector3.right);
        return ( Vector3.Dot(Vector3.right,returnHorizontalDirection) / Vector3.Dot(Vector3.back, returnHorizontalDirection) )* depthVelocity;   // A tester
    }

    [PunRPC]
    private void NetworkSwitchTarget()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Ajouter un timer pour eviter les double siwtch
            //SwitchTarget();
            view.RPC("SwitchTarget", RpcTarget.All);
        }
        else
        {
            view.RPC("NetworkSwitchTarget", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void SwitchTarget()
    {
        if (currentTarget == Target.PLAYER1)
            currentTarget = Target.PLAYER2;
        else if(currentTarget == Target.PLAYER2)
            currentTarget = Target.PLAYER1;
        //currentTarget = (Target)(((int)currentTarget + 1)%numberOfPlayer); 
    }

    private Vector3 GetCurrentTargetPosition()
    {
        if(currentTarget == Target.PLAYER1)
        {
            return playerTransforms[0].position;
        }
        else
        {
            return playerTransforms[1].position;
        }
        //return playerTransform[(int)playerTransforms].position;
    }

    private Vector3 GetCurrentTargetPositionX()
    {
        if (currentTarget == Target.PLAYER1)
        {
            return xReturnsPoints[0].position;
        }
        else
        {
            return xReturnsPoints[0].position;
        }
        //return playerTransform[(int)playerTransforms].position;
    }

    private Vector3 GetCurrentTargetPositionZ()
    {
        return zReturnPoints.position;
        //return playerTransform[(int)playerTransforms].position;
    }



    //////////////////////////////////////////    Racket Interraction     /////////////////////////////////////////////////

    [PunRPC]
    private Vector3 RacketArcadeHit()
    {
        return RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity();
    }

    [PunRPC]
    private Vector3 RacketBasicPhysicHit(Collision collision)       // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        Vector3 relativeVelocity = lastVelocity - racketVelocity;
        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;
        Vector3 tangentVelocity = (relativeVelocity - normalVelocity) * (1 - racketFriction);        // Ajouter frottement

        return -normalVelocity + tangentVelocity;
    }

    [PunRPC]
    private Vector3 RacketMediumPhysicHit(Collision collision) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = (2 * Vector3.Dot(contactPointNormal, racketVelocity) - Vector3.Dot(contactPointNormal, lastVelocity)) * contactPointNormal;
        Vector3 tangentVelocity = (lastVelocity - Vector3.Dot(contactPointNormal, lastVelocity) * contactPointNormal) * (1 - racketFriction);        // Ajouter frottement

        return normalVelocity + tangentVelocity;
    }

    [PunRPC]
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

    //////////////////////////////////////////    Utility Methods     /////////////////////////////////////////////////

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

    public int GetBallsColor()
    {
        return colorID;
    }

    public void SetBallColor(int colorID)
    {
        this.colorID = colorID;
        renderer.material = materials[colorID];
    }

    private void SwitchColor()
    {
        //ColorManager.instance.SwitchBallColor();
        colorID = (colorID + 1) % materials.Length;
        renderer.material = materials[colorID];
    }

    private void InitialiseTargetSwitchType()
    {
        if (switchType == TargetSwitchType.RACKETBASED)
        {
            switchIsRacketBased = true;
        }
        else
        {
            switchIsRacketBased = false;
        }
    }

    //#region IPunObservable implementation
    //void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(transform.position);
    //        stream.SendNext(transform.rotation);

    //    }
    //    else
    //    {
    //        transform.position = (Vector3)stream.ReceiveNext();
    //        transform.rotation = (Quaternion)stream.ReceiveNext();
    //    }
    //}
    //#endregion
}
