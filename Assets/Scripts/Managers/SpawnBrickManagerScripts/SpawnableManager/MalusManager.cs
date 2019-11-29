using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalusManager : SpawnableManager
{
    public static MalusManager instance;

    public override void Awake()
    {
        instance = this;
    }

    public override void SpawnRandomObject(Transform spawnTransform)
    {
        base.SpawnRandomObject(spawnTransform);
        Debug.Log("Spawn Malus");
    }
}
