using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] string[] spawnableObjectPoolNames;

    public virtual void Awake()
    {
        
    }

    public virtual void SpawnRandomObject(Transform spawnTransform)
    {
        int l_randomName = Random.Range(0, spawnableObjectPoolNames.Length);
        PoolManager.instance?.SpawnFromPool(spawnableObjectPoolNames[l_randomName], spawnTransform.position, spawnTransform.rotation);
    }
}
