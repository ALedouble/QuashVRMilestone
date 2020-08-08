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
    [SerializeField, ReadOnly] protected bool isAnimating;

    [Header("Bloom settings")]
    [SerializeField, ReadOnly] protected Bloom bloom;
    [SerializeField] protected AnimationCurve bloomEvolution;
    [SerializeField, ReadOnly] protected float minBloomIntensity;
    [SerializeField] protected float maxBloomIntensity;

    private float effectiveMaxBloomIntensity;

    protected virtual void Start()
    {
        bloom = volume.profile.GetSetting<Bloom>();
        minBloomIntensity = bloom.intensity.value;

        if (PlayerSettings.Instance?.FlashIntensity != null)
            effectiveMaxBloomIntensity = maxBloomIntensity * PlayerSettings.Instance.FlashIntensity;
        else
            effectiveMaxBloomIntensity = maxBloomIntensity;
    }

    protected virtual void Update()
    {
        Animation();
    }

    [Button]
    protected virtual void StartAnim()
    {
        animCount = 0.0f;
        isAnimating = true;
    }

    protected virtual void Animation()
    {
        if (!isAnimating) return;
        animCount += Time.deltaTime;
        bloom.intensity.value = Mathf.Lerp(minBloomIntensity, effectiveMaxBloomIntensity, bloomEvolution.Evaluate(AlphaCount()));

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
