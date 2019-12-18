using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFlash_Brick : PostProcessFlash
{
    protected override void Start()
    {
        base.Start();
        BallEventManager.instance.OnCollisionWithBrick += StartAnim;
    }
}
