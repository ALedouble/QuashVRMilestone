using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BallCollisionFX : MonoBehaviour
{
    private Vector3 impactPosition;

    public float cooldownBetweenTwoImpactFX;
    private float lastExplosionTime = -10f;         //Safety Value
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(GameManager.Instance.offlineMode || PhotonNetwork.IsMasterClient)
        {
            impactPosition = collision.GetContact(0).point;

            if (collision.gameObject.tag == "Wall")
            {
                FXManager.Instance.PlayWallBounceFX(impactPosition, collision.contacts[0].normal);                                  //Need Mise en reseau(RPC)
            }
            else if (collision.gameObject.tag == "FrontWall")
            {
                ExecuteFrontWallBrickInterraction(collision);
            }
            else if (collision.gameObject.tag == "Brick")
            {
                ExecuteFrontWallBrickInterraction(collision);
                BrickInfo brickInfo = collision.gameObject.GetComponent<BrickInfo>();

                if (brickInfo.colorID == 0 || brickInfo.colorID == BallManager.instance.GetBallColorID() + 1)
                {
                    BrickManager.Instance.HitBrickByID(collision.gameObject.GetComponent<BrickBehaviours>().BrickID);
                }
            }
        }
    }

    private void ExecuteFrontWallBrickInterraction(Collision collision)
    {
        if (Time.time > lastExplosionTime + cooldownBetweenTwoImpactFX)
        {
            ScoreManager.Instance.CheckForComboBreak();

            Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);
            FXManager.Instance.PlayExplosion(pos, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
            //CallForExplosion(collision);
        }
    }

    //private void CallForExplosion(Collision collision)
    //{
    //    Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);


    //    FXManager.Instance.PlayExplosion(pos, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
    //    //if (GameManager.Instance.offlineMode)                                                                                                              // Faire remonter pour englobé le score?
    //    //{
    //    //    FXManager.Instance.PlayExplosion(pos, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
    //    //}
    //    //else /*if (PhotonNetwork.IsMasterClient)*/
    //    //{
    //    //    FXManager.Instance.PlayExplosion(pos, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
    //    //}
    //}
}
