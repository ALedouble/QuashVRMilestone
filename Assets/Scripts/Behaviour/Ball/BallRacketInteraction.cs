﻿using System.Collections;
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

    [Range(0, 1)]
    public float ballSpeedFactorRatio;

    [Range(0, 1)]
    public float racketFriction;

    [Range(0, 1)]
    public float mixRatio;


    private float minVib = 0.0006f;
    private float maxVib = 0.025f;

    private float minHit = 3.3f;
    private float maxHit = 50f;

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
            RacketInteraction(other);
            //Debug.Log("LAST Velocity : " + ballPhysicBehaviour.LastVelocity.magnitude);
            
            float ballVelocity = ballPhysicBehaviour.LastVelocity.magnitude;
            float hitRange = maxHit - minHit;
            float vibRange = maxVib - minVib;
            float hitPercent;
            float hitRate;
            float vib;


            if (ballVelocity <= minHit)
            {
                VibrationManager.instance.VibrateOn("Vibration_Racket_Hit", minVib); hitRate = 0.01f; //Debug.Log("VIB percent : " + vibPercent);
            }
            else if (ballVelocity >= maxHit)
            {
                VibrationManager.instance.VibrateOn("Vibration_Racket_Hit", maxVib); hitRate = 1f; //Debug.Log("VIB percent : " + vibPercent);
            }
            else
            {
                hitPercent = ((ballVelocity - minHit) * 0.01f) / hitRange;
                vib = ((hitPercent * vibRange) * 0.01f) + minVib;
                VibrationManager.instance.VibrateOn("Vibration_Racket_Hit", vib);

                //Debug.Log("hit percent : " + hitPercent);
                hitRate = hitPercent * 100f;
                //Debug.Log("Hit RATE : " + hitRate);
                //Debug.Log("VIB : " + vib);
            }

            

            AudioManager.instance.PlaySound("RacketHit", other.GetContact(0).point, hitRate);
            //AudioManager.instance.PlaySound("RacketHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude);

            RacketManager.instance.racketPostProcess.bloomPercent = hitRate;
            BallEventManager.instance.OnBallCollision("Racket");
        }

    }

    #region RacketInteraction

    private void RacketInteraction(Collision other)
    {
        ApplyRacketPhysic(other);

        RacketManager.instance.OnHitEvent(gameObject);  // Ignore collision pour quelques frames.

        SetLastPlayerWhoHitTheBall();
        SwitchTarget();
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
            ballPhysicBehaviour.ApplyNewVelocity(newVelocity * ballPhysicBehaviour.globalSpeedMultiplier, transform.position, (int)SpeedState.NORMAL, false);                                  // Modif globalSpeedMultiplier
        }
        else
        {
            //Marche pas!
            photonView.RPC("ApplyNewVelocity", RpcTarget.All, newVelocity * ballPhysicBehaviour.globalSpeedMultiplier, transform.position, (int)SpeedState.NORMAL, false);     // Modif globalSpeedMultiplier
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

    #region Racket Physics

    private Vector3 RacketArcadeHit()
    {
        return RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity();
    }

    private Vector3 RacketBasicPhysicHit(Collision collision)       // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.localPlayerRacket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        //Vector3 relativeVelocity = ballPhysicBehaviour.LastVelocity - racketVelocity;
        Vector3 relativeVelocity = racketVelocity - ballPhysicBehaviour.LastVelocity * ballSpeedFactorRatio;
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

    private Vector3 RacketMixedHit(Collision collision)
    {
        return RacketArcadeHit() * (1 - mixRatio) + RacketBasicPhysicHit(collision) * mixRatio;
    }

    #endregion

    #endregion

    #region Multi MidWallStatus

    private void SetMidWallStatus(bool isCollidable)
    {
        if (GameManager.Instance.offlineMode)
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
        if (!GameManager.Instance.offlineMode)
            photonView.RPC("SwitchTargetRPC", RpcTarget.All);
    }

    [PunRPC]
    private void SwitchTargetRPC()
    {
        targetSelector.SwitchTarget();
    }

    #endregion

    #region LastPlayerWhoHitTheBall

    private void SetLastPlayerWhoHitTheBall()
    {
        if (GameManager.Instance.offlineMode)
        {
            ballInfo.LastPlayerWhoHitTheBall = QPlayer.PLAYER1;
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
        ballInfo.LastPlayerWhoHitTheBall = QPlayer.PLAYER1;
    }

    [PunRPC]
    private void SetLastPlayerToHitTheBallToPlayer2()
    {
        ballInfo.LastPlayerWhoHitTheBall = QPlayer.PLAYER2;
    }

    #endregion

}
