using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFlash_BallMissed : PostProcessFlash
{
    [SerializeField] Color bloomTargetColor = Color.red;
    [SerializeField] Color bloomIniColor;

    protected override void Start()
    {
        base.Start();
        bloomIniColor = bloom.color.value;
    }

    protected override void StartAnim()
    {
        base.StartAnim();
        //maxBloomIntensity = minBloomIntensity;
    }

    protected override void Animation()
    {
        if (!isAnimating) return;
        base.Animation();
        bloom.color.value = Color.Lerp(bloomIniColor, bloomTargetColor, bloomEvolution.Evaluate(AlphaCount()));
    }
}
