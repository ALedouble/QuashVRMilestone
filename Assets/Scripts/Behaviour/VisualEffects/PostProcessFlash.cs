using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using NaughtyAttributes;

public class PostProcessFlash : MonoBehaviour
{
    [SerializeField] PostProcessVolume volume;
    [SerializeField] float animDuration = 1.0f;
    [SerializeField, ReadOnly] float animCount = 0.0f;
    [SerializeField, ReadOnly] bool isAnimating;

    [Header("Bloom settings")]
    [SerializeField, ReadOnly] Bloom bloom;
    [SerializeField] AnimationCurve bloomEvolution;
    [SerializeField, ReadOnly] float minBloomIntensity;
    [SerializeField] float maxBloomIntensity;

    protected virtual void Start()
    {
        bloom = volume.profile.GetSetting<Bloom>();
        minBloomIntensity = bloom.intensity.value;
    }

    protected virtual void Update()
    {
        Animation();
    }

    protected virtual void StartAnim(BallCollisionInfo ballCollisionInfo)
    {
        animCount = 0.0f;
        isAnimating = true;
    }

    void Animation()
    {
        if (!isAnimating) return;
        animCount += Time.deltaTime;
        bloom.intensity.value = Mathf.Lerp(minBloomIntensity, maxBloomIntensity, bloomEvolution.Evaluate(AlphaCount()));

        if(AlphaCount() >= 1.0f)
        {
            isAnimating = false;
        }
    }

    protected float AlphaCount()
    {
        return animCount / animDuration;
    }
}
