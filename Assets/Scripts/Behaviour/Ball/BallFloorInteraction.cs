using System.Collections;
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
            if(GameManager.Instance.offlineMode || BallMultiplayerBehaviour.Instance.IsBallOwner)
                StartBallResetCountdown();
            
            StandardBounce(other.GetContact(0));

            SendFeedback(other.GetContact(0).point, other.GetContact(0).normal);

            BallEventManager.instance.OnBallCollision("Floor", other);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Floor")
            BallEventManager.instance.OnBallCollisionExit("Floor");
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

        ballPhysicBehaviour.PhysicRelatedVelocityUpdate(((1 - ballPhysicBehaviour.dynamicFriction) * tangentVelocity * tangent - ballPhysicBehaviour.bounciness * normalVelocity * normal));
    }

    private void SendFeedback(Vector3 contactPoint, Vector3 normal)
    {
        if (GameManager.Instance.offlineMode)
        {
            PlayFeedback(contactPoint, normal);
        }
        else if (BallMultiplayerBehaviour.Instance.IsBallOwner)
        {
            photonView.RPC("PlayFeedback", RpcTarget.All, contactPoint, normal);
        }
    }

    [PunRPC]
    private void PlayFeedback(Vector3 contactPoint, Vector3 normal)
    {
        AudioManager.instance.PlaySound("FloorHit", contactPoint, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude);
        FXManager.Instance.PlayWallBounceFX(contactPoint, normal);
    }

    #region ResetImmobileBall

    public void StartBallResetCountdown()
    {
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

    private void StopBallResetCountdown(Collision collision)
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

        if(!LevelManager.instance.currentLevel.level.levelSpec.suddenDeath)
            ScoreManager.Instance.ResetCombo((int)BallManager.instance.GetPlayerWhoLostTheBall());

        Debug.Log("BallResetCoroutine: LoseBall");
    }

    #endregion

}
