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

    private void Start()
    {
        bloom = volume.profile.GetSetting<Bloom>();
        minBloomIntensity = bloom.intensity.value;
        BallEventManager.instance.OnCollisionWithRacket += StartAnim;
    }

    private void Update()
    {
        Animation();
    }

    [Button]
    void StartAnim()
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

    float AlphaCount()
    {
        return animCount / animDuration;
    }
}
