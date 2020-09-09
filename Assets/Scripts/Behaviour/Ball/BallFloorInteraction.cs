﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallFloorInteraction : MonoBehaviour
{
    public float delayBeforeReset;

    private BallPhysicBehaviour ballPhysicBehaviour;

    private Coroutine resetCoroutine;

    private PhotonView photonView;


    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();

        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "Floor")
        {
            StartBallResetCountdown();
            StandardBounce(other.GetContact(0));
            // Sound Magnitude TO BE FIX !!!
            AudioManager.instance.PlaySound("FloorHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude);

            SendBallCollisionEvent("Floor");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Floor")
            SendBallCollisionExitEvent("Floor");
    }

    

    /// Méthode qui calcul le rebond de la balle (calcul vectorielle basique) et modifie la trajectoire en conséquence
    /// contactPoint : données de collision entre la balle et l'autre objet
    private void StandardBounce(ContactPoint contactPoint)
    {
        Vector3 normal = Vector3.Normalize(contactPoint.normal);
        float normalVelocity = Vector3.Dot(normal, ballPhysicBehaviour.LastVelocity);
        if (normalVelocity > 0)
            normalVelocity = -normalVelocity;

        Vector3 tangent = Vector3.Normalize(ballPhysicBehaviour.LastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, ballPhysicBehaviour.LastVelocity);

        ballPhysicBehaviour.ApplyNewVelocity(((1 - ballPhysicBehaviour.dynamicFriction) * tangentVelocity * tangent - ballPhysicBehaviour.bounciness * normalVelocity * normal));
    }

    //Need Rework!!!
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

    #region ResetImmobileBall

    public void StartBallResetCountdown()
    {
        Debug.Log("StartBallResetCountdown");
        if (GameManager.Instance.offlineMode || PhotonNetwork.IsMasterClient)
        {
            BallEventManager.instance.OnCollisionExitWithFloor += StopBallResetCountdown;
            BallEventManager.instance.OnCollisionWithBackWall += StopBallResetCountdown;

            resetCoroutine = StartCoroutine(BallResetCoroutine());
        }
    }

    private void StopBallResetCountdown()
    {
        BallEventManager.instance.OnCollisionExitWithFloor -= StopBallResetCountdown;
        BallEventManager.instance.OnCollisionWithBackWall -= StopBallResetCountdown;

        StopCoroutine(resetCoroutine);
    }

    private IEnumerator BallResetCoroutine()
    {
        float resetTimer = 0;
        while (resetTimer < delayBeforeReset)
        {
            yield return new WaitForFixedUpdate();

            if (!BallManager.instance.IsBallPaused)
                resetTimer += Time.fixedDeltaTime;
        }

        BallEventManager.instance.OnCollisionWithBackWall -= StopBallResetCountdown;
        BallManager.instance.LoseBall();
        Debug.Log("BallResetCoroutine: LoseBall");
    }

    #endregion

}
