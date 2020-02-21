using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFlash_ComboBroken : PostProcessFlash_BallMissed
{
    protected override void Start()
    {
        base.Start();
        BallEventManager.instance.OnCollisionWithBackWall -= StartAnim;
        ScoreManager.Instance.OnComboReset += StartAnim;
    }
}
