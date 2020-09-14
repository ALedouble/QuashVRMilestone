using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFlash_Racket : PostProcessFlash
{
    public float bloomPercent;

    protected override void Start()
    {
        base.Start();
        BallEventManager.instance.OnCollisionWithRacket += StartAnim;
    }

    protected override void Animation()
    {
        if (!isAnimating) return;
        animCount += Time.deltaTime;
        bloom.intensity.value = Mathf.Lerp(minBloomIntensity, effectiveMaxBloomIntensity * bloomPercent, bloomEvolution.Evaluate(AlphaCount()));

        if (AlphaCount() >= 1.0f)
        {
            isAnimating = false;
            bloomPercent = 1;
        }
    }
}
