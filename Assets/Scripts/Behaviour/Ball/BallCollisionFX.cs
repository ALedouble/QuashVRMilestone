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

            //Debug.DrawRay(this.transform.position, collision.contacts[0].normal, Color.blue, 20);
            //Debug.DrawRay(this.transform.position, Vector3.up, Color.green, 20);
        }

        if (collision.gameObject.tag == "Brick" || collision.gameObject.tag == "FrontWall")
        {
            currentCooldown = 0;

            Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, collision.gameObject.transform.position.z);
            Debug.Log("Brick me");

            if (canSpawn)
            {
                //Debug.Log("Spawn an impact");
                //GameObject obj = PoolManager.instance.SpawnFromPool("ImpactFX", pos, Quaternion.identity);
                //DebugManager.Instance.DisplayValue(0, collision.impulse.magnitude.ToString());
                //obj.GetComponent<BallImpactDestruction>().maxRadius = collision.impulse.magnitude;
                Debug.Log("Set explosion");

                FXManager.Instance.SetExplosion(pos, collision.relativeVelocity.magnitude);

                canSpawn = false;
            }
        }

        if (collision.collider.TryGetComponent<IBrick>(out IBrick brick))
        {
            BrickManager.Instance.DeadBrick(brick.GetBrickInfo());
        }
    }


}
