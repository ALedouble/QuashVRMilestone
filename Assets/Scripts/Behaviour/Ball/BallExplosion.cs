using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallExplosion : MonoBehaviour
{
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        BallEventManager.instance.OnCollisionWithBrick += CreateExplosion;
        BallEventManager.instance.OnCollisionWithFrontWall += CreateExplosion;
    }

    private void CreateExplosion(Collision collision)
    {
        ScoreManager.Instance.CheckForComboBreak();

        Vector3 impactPosition = collision.GetContact(0).point;

        Vector3 position = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);
        ExplosionManager.Instance.CreateExplosion(position, (int)QPlayerManager.instance.LocalPlayerID);
    }
}
