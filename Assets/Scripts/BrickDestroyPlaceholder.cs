using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickDestroyPlaceholder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Ball")
        //{
        //    gameObject.SetActive(false);
        //    PoolManager.instance.SpawnFromPool("CubeImpactFX", transform.position, Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up));
        //    PoolManager.instance.SpawnFromPool("ImpactFX", transform.position, Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up));
        //}
    }
}
