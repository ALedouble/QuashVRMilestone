using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Shaker : MonoBehaviour
{
    [SerializeField] Shake defaultShake;

    float count;
    float frequencyCount;
    Shake currentShake;
    Vector3 targetPos;
    Vector3 originPos;
    Vector3 startPos;
    bool isShaking;

    private void Start()
    {
        originPos = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) PlayDefaultShake(); //Debug
        Shaking();
    }

    [Button]
    public void PlayDefaultShake()
    {
        StartShake(defaultShake);
    }

    public void PlayShake(Shake p_shake)
    {
        StartShake(p_shake);
    }

    public void PlayShake(float duration = 1, float magnitude = 0.25f, float frequency = 0.1f)
    {
        Keyframe keyframe0 = new Keyframe(0, 1);
        Keyframe keyframe1 = new Keyframe(1, 1);
        Keyframe[] keyframes = new Keyframe[] { keyframe0, keyframe1 };
        AnimationCurve curve = new AnimationCurve(keyframes);
        Shake shake = new Shake(curve, duration, magnitude, frequency);

        StartShake(shake);
    }

    void StartShake(Shake p_shake)
    {
        count = 0;
        frequencyCount = 0;
        startPos = originPos;
        targetPos = originPos + GetRandomPos(p_shake);
        currentShake = p_shake;
        isShaking = true;
    }


    void Shaking()
    {
        if (!isShaking) return;

        count += Time.deltaTime;
        frequencyCount += Time.deltaTime;

        float alphaFrequency = frequencyCount / currentShake.ShakeFrequency;
        float alphaLifetime = count / currentShake.LifeTime;

        transform.localPosition = Vector3.Lerp(startPos, targetPos, alphaFrequency); //Lerp

        if (alphaFrequency >= 1)
        {
            targetPos = originPos + GetRandomPos(currentShake, alphaLifetime);
            startPos = transform.localPosition;
            frequencyCount = 0.0f;
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
        if (p_shake.ShakeY) z = Random.Range(-1f, 1f) * p_shake.GetMagnitude(time);

        return new Vector3(x, y, z);
    }

}
