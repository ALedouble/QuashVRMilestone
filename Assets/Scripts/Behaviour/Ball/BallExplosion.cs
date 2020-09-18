using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallExplosion : MonoBehaviour
{
    private Vector3 impactPosition;

    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!BallManager.instance.IsBallPaused)
        {
            impactPosition = collision.GetContact(0).point;

            if (collision.gameObject.tag == "FrontWall")
            {
                CreateExplosion(collision);
            }
            else if (collision.gameObject.tag == "Brick")
            {
                CreateExplosion(collision);
                BrickInfo brickInfo = collision.gameObject.GetComponent<BrickInfo>();

                // !!! Rework Explosion !!!
                if (brickInfo.colorID == 0 || brickInfo.colorID == BallManager.instance.GetBallColorID())
                {
                    BrickDestructionManager.Instance.HitBrickByID(collision.gameObject.GetComponent<BrickInfo>().BrickID, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
                }
            }
        }
    }

    private void CreateExplosion(Collision collision)
    {
        ScoreManager.Instance.CheckForComboBreak();

        Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);
        ExplosionManager.Instance.CreateExplosion(pos, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
    }
}
