using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFlash_Racket : PostProcessFlash
{
    protected override void Start()
    {
        base.Start();
        BallEventManager.instance.OnCollisionWithRacket += StartAnim;
    }
}
