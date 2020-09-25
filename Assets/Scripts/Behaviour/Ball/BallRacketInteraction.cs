using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum RacketInteractionType
{
    BASICARCADE,
    BASICPHYSIC,
    MEDIUMPHYSIC,
    MIXED
}

public class BallRacketInteraction : MonoBehaviour
{
    public RacketInteractionType physicsUsed;

    // Animation curve
    public float hitMaxSpeed;
    public float hitMinSpeed;
    public float hitSpeedMultiplier;

    public AnimationCurve racketVibrationCurve;

    [Range(0, 1)]
    public float ballSpeedWeight;

    [Range(0, 1)]
    public float racketFriction;

    private BallPhysicBehaviour ballPhysicBehaviour;
    private BallInfo ballInfo;
    private ITargetSelector targetSelector;

    private PhotonView photonView;


    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();
        ballInfo = GetComponent<BallInfo>();
        targetSelector = GetComponent<ITargetSelector>();

        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "Racket")
        {
            Vector3 ballNewVelocity = RacketInteraction(other);

            SendFeedback(other.GetContact(0).point, ballNewVelocity);
            
            BallEventManager.instance.OnBallCollision("Racket", other);
        }
    }

    private float GetCurrentHitRateAtCollision(Vector3 ballNewVelocity)
    {
        float ballVelocityRate = (ballNewVelocity.magnitude / hitMaxSpeed);

        return ballVelocityRate;
    }

    #region RacketInteraction

    private Vector3 RacketInteraction(Collision other)
    {
        Vector3 ballNewVelocity = ApplyRacketPhysic(other);

        RacketManager.instance.OnHit(gameObject);  // Ignore collision pour quelques frames.

        UpdateLastPlayerWhoHitTheBall();
        SwitchTarget();
        SetMidWallStatus(true);

        return ballNewVelocity;
    }

    private Vector3 ApplyRacketPhysic(Collision other)
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
        }

        newVelocity = ClampVelocity(hitSpeedMultiplier * newVelocity);

        ballPhysicBehaviour.OverrideRawVelocity(newVelocity, (int)SpeedState.NORMAL, false);

        return newVelocity;
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

    #region Racket Physics

    private Vector3 RacketArcadeHit()
    {
        return RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity();
    }

    private Vector3 RacketBasicPhysicHit(Collision collision)       // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        //Vector3 relativeVelocity = ballPhysicBehaviour.LastVelocity - racketVelocity;
        Vector3 relativeVelocity = racketVelocity - ballPhysicBehaviour.LastVelocity * ballSpeedWeight;
        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;
        Vector3 tangentVelocity = (relativeVelocity - normalVelocity) * (1 - racketFriction);

        return normalVelocity + tangentVelocity;
    }

    private Vector3 RacketMediumPhysicHit(Collision collision) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = (2 * Vector3.Dot(contactPointNormal, racketVelocity) - Vector3.Dot(contactPointNormal, ballPhysicBehaviour.LastVelocity)) * contactPointNormal;
        Vector3 tangentVelocity = (ballPhysicBehaviour.LastVelocity - Vector3.Dot(contactPointNormal, ballPhysicBehaviour.LastVelocity) * contactPointNormal) * (1 - racketFriction);        // Ajouter frottement

        return normalVelocity + tangentVelocity;
    }

    #endregion

    #endregion

    //A bouger!
    #region Multi MidWallStatus

    private void SetMidWallStatus(bool isCollidable)
    {
        if (!GameManager.Instance.offlineMode)
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

    #region ReturnTarget

    private void SwitchTarget()
    {
        targetSelector.SwitchTarget();
    }

    #endregion

    #region Feedback

    private void SendFeedback(Vector3 contactPoint, Vector3 ballNewVelocity)
    {
        float hitRate = GetCurrentHitRateAtCollision(ballNewVelocity);

        //Debug.Log("hit rate : " + hitRate);
        float vib = racketVibrationCurve.Evaluate(hitRate);
        
        VibrationManager.instance.VibrateOn("Vibration_Racket_Hit", vib);

        if (GameManager.Instance.offlineMode)
            PlayFeedback(contactPoint, hitRate);
        else
            photonView.RPC("PlayFeedback", RpcTarget.All, "RacketHit", contactPoint, hitRate);
    }

    [PunRPC]
    private void PlayFeedback(Vector3 contactPoint, float intensity)
    {
        AudioManager.instance.PlaySound("RacketHit", contactPoint, intensity);
        RacketManager.instance.racketPostProcess.bloomPercent = intensity;
    }

    #endregion

    #region LastPlayerWhoHitTheBall

    private void UpdateLastPlayerWhoHitTheBall()
    {
        if (GameManager.Instance.offlineMode)
        {
            ballInfo.LastPlayerWhoHitTheBall = QPlayer.PLAYER1;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ballInfo.LastPlayerWhoHitTheBall = QPlayer.PLAYER1;
            }
            else
            {
                ballInfo.LastPlayerWhoHitTheBall = QPlayer.PLAYER2;
            }
        }
    }

    #endregion

}
