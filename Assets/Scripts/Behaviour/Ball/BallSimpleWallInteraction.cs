using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallSimpleWallInteraction : MonoBehaviour
{
    private BallPhysicBehaviour ballPhysicBehaviour;

    private PhotonView photonView;


    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();

        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "Wall")
        {
            StandardBounce(other.GetContact(0));
            // Sound Magnitude TO BE FIX
            SendFeedback(other.GetContact(0).point, other.contacts[0].normal);

            BallEventManager.instance.OnBallCollision("Wall", other);
        }
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
        if(GameManager.Instance.offlineMode)
        {
            PlaySimpleWallCollisionFeedback(contactPoint, normal);
        }
        else if(BallMultiplayerBehaviour.Instance.IsBallOwner)
        {
            photonView.RPC("PlaySimpleWallCollisionFeedback", RpcTarget.All, contactPoint, normal);
        }
    }

    [PunRPC]
    private void PlaySimpleWallCollisionFeedback(Vector3 contactPoint, Vector3 normal)
    {
        AudioManager.instance.PlaySound("WallHit", contactPoint, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude);
        FXManager.Instance.PlayWallBounceFX(contactPoint, normal);
    }
}
