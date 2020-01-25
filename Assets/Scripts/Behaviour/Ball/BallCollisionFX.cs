using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BallCollisionFX : MonoBehaviour
{
    private Vector3 impactPosition;

    private float currentCooldown = 0f;
    public float cooldownBetweenTwoImpactFX;
    private bool canSpawn = false;
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();    
    }

    private void Update()
    {
        if (currentCooldown < cooldownBetweenTwoImpactFX)
        {
            currentCooldown += Time.deltaTime;
        }
        else
        {
            canSpawn = true;
        }
    }
    
    [PunRPC]
    void SetExplosionRPC(Vector3 position, float magnitude, int lastPlayer) {
        FXManager.Instance.SetExplosion(position, magnitude, lastPlayer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        impactPosition = collision.GetContact(0).point;

        if (collision.gameObject.tag == "Wall")
        {
           //Vector3 forward = Vector3.Cross(collision.contacts[0].normal, collision.transform.up);
            PoolManager.instance.SpawnFromPool("BounceFX", impactPosition,
                Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up));
        }

        if(collision.gameObject.tag == "FrontWall")
        {
            CallExplosion(collision);
        }

        if (collision.gameObject.tag == "Brick"  )
        {
            CallExplosion(collision);
            BrickInfo brickInfo = collision.gameObject.GetComponent<BrickInfo>();

            if (brickInfo.colorID == 0 || brickInfo.colorID == BallManager.instance.GetBallColorID() + 1)
            {
                BrickManager.Instance.HitBrickByID(collision.gameObject.GetComponent<BrickBehaviours>().BrickID);
            }
        }

        
    }

    private void CallExplosion(Collision collision)
    {
        ScoreManager.Instance.resetCombo = true;

        StartCoroutine(CheckComboCondition(FXManager.Instance.impactMaxTime, (int)BallManager.instance.GetLastPlayerWhoHitTheBall())); //BallID

        currentCooldown = 0;

        Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);

        if (canSpawn)
        {
            //BallID
            if (PhotonNetwork.OfflineMode)
            {
                FXManager.Instance.SetExplosion(pos, collision.relativeVelocity.magnitude, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC("SetExplosionRPC", RpcTarget.All, pos, collision.relativeVelocity.magnitude, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
                FXManager.Instance.SetExplosion(pos, collision.relativeVelocity.magnitude, (int)BallManager.instance.GetLastPlayerWhoHitTheBall());
            }

            canSpawn = false;
        }
    }


    public IEnumerator CheckComboCondition(float timeBeforeCheck, int playerID)
    {
        yield return new WaitForSeconds(timeBeforeCheck);

        if (ScoreManager.Instance.resetCombo)
        {
            ScoreManager.Instance.ResetCombo(playerID);
        }
    }
}
