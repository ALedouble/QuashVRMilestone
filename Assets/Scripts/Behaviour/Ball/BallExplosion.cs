using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallExplosion : MonoBehaviour
{
    private Vector3 impactPosition;

    public float cooldownBetweenTwoImpactFX;
    private float lastExplosionTime = -10f;         //Safety Value
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // !!! Rework Explosion !!!
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

                if (brickInfo.colorID == 0 || brickInfo.colorID == BallManager.instance.GetBallColorID())
                {
                    BrickManager.Instance.HitBrickByID(collision.gameObject.GetComponent<BrickInfo>().BrickID);
                }
            }
        }
    }

    private void CreateExplosion(Collision collision)
    {
        if (Time.time > lastExplosionTime + cooldownBetweenTwoImpactFX)
        {
            ScoreManager.Instance.CheckForComboBreak();

            Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);
            ExplosionManager.Instance.CreateExplosion(pos, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
        }
    }
}
