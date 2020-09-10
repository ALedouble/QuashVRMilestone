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
            AudioManager.instance.PlaySound("WallHit", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude);
            FXManager.Instance.PlayWallBounceFX(other.GetContact(0).point, other.contacts[0].normal);

            BallEventManager.instance.OnBallCollision("Wall");
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

            ballPhysicBehaviour.ApplyNewVelocity(((1 - ballPhysicBehaviour.dynamicFriction) * tangentVelocity * tangent - ballPhysicBehaviour.bounciness * normalVelocity * normal));
    }
}
