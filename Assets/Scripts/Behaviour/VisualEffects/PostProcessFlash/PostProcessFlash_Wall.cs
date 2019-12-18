using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFlash_Wall : PostProcessFlash
{
    protected override void Start()
    {
        base.Start();
        BallEventManager.instance.OnCollisionWithWall += StartAnim;
    }
}
