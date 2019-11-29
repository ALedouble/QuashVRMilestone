using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : SpawnableManager
{
    public static BonusManager instance;

    public override void Awake()
    {
        instance = this;
    }

    public override void SpawnRandomObject(Transform spawnTransform)
    {
        base.SpawnRandomObject(spawnTransform);
        Debug.Log("Spawn Bonus");
    }
}
