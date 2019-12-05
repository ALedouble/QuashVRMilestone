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
            PoolManager.instance.SpawnFromPool("BounceFX", impactPosition, collision.gameObject.transform.rotation);
        }

        if (collision.gameObject.tag == "Brick" || collision.gameObject.tag == "FrontWall")
        {
            currentCooldown = 0;

            Vector3 pos = new Vector3(impactPosition.x, impactPosition.y, 24.4f);

            if (canSpawn)
            {
                Debug.Log("Spawn an impact");
                PoolManager.instance.SpawnFromPool("ImpactFX", pos, Quaternion.identity);
                
                canSpawn = false;
            }
        }
    }


}
