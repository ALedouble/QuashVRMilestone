using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Shaker : MonoBehaviour
{
    [SerializeField] Shake defaultShake;

    float count;
    float frequencyCount;
    float frequencyTarget;
    float magnitudeScale = 1;
    Shake currentShake;
    Vector3 targetPos;
    Vector3 originPos;
    Vector3 startPos;
    bool isShaking;

    [SerializeField, Slider(0.0f, 1.0f)] float unstoppableShakingLimit = 0.35f;

    private void Start()
    {
        originPos = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) PlayShake(); //Debug
        Shaking();
    }

    [Button]
    public void PlayShake()
    {
        if (defaultShake == null) return;
        StartShake(defaultShake);
    }

    public void PlayShake(Shake p_shake, float p_magnitudeScale = 1)
    {
        Debug.Log("Start Shake of : " + gameObject.name);
        StartShake(p_shake,p_magnitudeScale);
    }

    public void PlayShake(float duration = 1, float magnitude = 0.25f, float frequency = 0.1f, float p_magnitudeScale = 1)
    {
        Keyframe keyframe0 = new Keyframe(0, 1);
        Keyframe keyframe1 = new Keyframe(1, 1);
        Keyframe[] keyframes = new Keyframe[] { keyframe0, keyframe1 };
        AnimationCurve shakeCurve = new AnimationCurve(keyframes);
        Shake shake = new Shake(shakeCurve, shakeCurve, duration, magnitude, frequency);
        StartShake(shake,p_magnitudeScale);
    }

    void StartShake(Shake p_shake, float p_magnitudeScale = 1)
    {
        if (isShaking)
        {
            float alphaLifetime = count / currentShake.LifeTime;
            if (alphaLifetime < unstoppableShakingLimit) return;
        }

        Debug.Log("Start Shake of : " + gameObject.name);
        magnitudeScale = p_magnitudeScale;
        count = 0;
        frequencyCount = 0;
        frequencyTarget = p_shake.GetFrequencyOverTime(0);
        startPos = originPos;
        targetPos = originPos + GetRandomPos(p_shake);
        currentShake = p_shake;
        isShaking = true;
    }


    void Shaking()
    {
        if (!isShaking) return;

        count += Time.deltaTime * currentShake.SpeedMultiplier;
        frequencyCount += Time.deltaTime * currentShake.SpeedMultiplier;

        float alphaLifetime = count / currentShake.LifeTime;

        
        float alphaFrequency = frequencyCount / frequencyTarget;
        
        transform.localPosition = Vector3.Lerp(startPos, targetPos, alphaFrequency); //Lerp

        if (alphaFrequency >= 1)
        {
            targetPos = originPos + (GetRandomPos(currentShake, alphaLifetime) * magnitudeScale);
            startPos = transform.localPosition;
            frequencyCount = 0.0f;
            frequencyTarget = currentShake.GetFrequencyOverTime(alphaLifetime);
            Debug.Log("SALOPE = " + currentShake.GetFrequencyOverTime(alphaLifetime));
        }

        if (alphaLifetime >= 1)
        {
            //End Shake
            transform.localPosition = originPos;
            isShaking = false;
        }

    }

    Vector3 GetRandomPos(Shake p_shake, float time = 0)
    {
        float x = 0;
        float y = 0;
        float z = 0;

        if (p_shake.ShakeX) x = Random.Range(-1f, 1f) * p_shake.GetMagnitude(time);
        if (p_shake.ShakeY) y = Random.Range(-1f, 1f) * p_shake.GetMagnitude(time);
        if (p_shake.ShakeZ) z = Random.Range(-1f, 1f) * p_shake.GetMagnitude(time);

        return new Vector3(x, y, z);
    }

}
