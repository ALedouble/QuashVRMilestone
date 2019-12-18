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

    private void OnCollisionEnter(Collision collision)
    {
        impactPosition = collision.GetContact(0).point;

        if (collision.gameObject.tag == "Wall")
        {
            Vector3 forward = Vector3.Cross(collision.contacts[0].normal, collision.transform.up);
            GameObject obj = PoolManager.instance.SpawnFromPool("BounceFX", impactPosition, 

                Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up));
        }

        if (collision.gameObject.tag == "Brick" || collision.gameObject.tag == "FrontWall")
        {
            currentCooldown = 0;

            Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);

            if (canSpawn)
            {
                FXManager.Instance.SetExplosion(pos, collision.relativeVelocity.magnitude);

                canSpawn = false;
            }
        }

        if (collision.collider.TryGetComponent<IBrick>(out IBrick brick))
        {
            //BrickManager.Instance.DeadBrick(brick.GetBrickInfo());
            collision.collider.gameObject.GetComponent<BrickBehaviours>().HitBrick();
        }
    }


}
