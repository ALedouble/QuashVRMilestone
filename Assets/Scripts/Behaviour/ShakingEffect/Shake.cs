using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ShakeAsset_00", menuName = "Custom/ShakeAsset", order = 1)]
public class Shake : ScriptableObject
{
    [SerializeField] AnimationCurve shakeOverLifetime;
    [SerializeField,MinValue(0.01f)] float lifeTime = 1;
    [SerializeField,MinValue(0.01f)] float shakeMagnitude = 1;
    [SerializeField] AnimationCurve frequencyOverLifetime;
    [SerializeField,Slider(0.0f,0.18f)] float shakeFrequency = 1;
    [SerializeField] bool shakeX = true;
    [SerializeField] bool shakeY = true;
    [SerializeField] bool shakeZ = true;

    public Shake(AnimationCurve p_shakeOverLifetime, AnimationCurve p_frequencyOverLifetime,float p_lifeTime,float p_shakeMagnitude, float p_shakeFrequency,bool p_shakeX = true,bool p_shakeY = true,bool p_shakeZ = true)
    {
        ShakeOverLifetime = p_shakeOverLifetime;
        FrequencyOverLifetime = p_frequencyOverLifetime;
        LifeTime = p_lifeTime;
        ShakeMagnitude = p_shakeMagnitude;
        ShakeFrequency = p_shakeFrequency;
        ShakeX = p_shakeX;
        ShakeY = p_shakeY;
        ShakeZ = p_shakeZ;
    }

    public float GetMagnitude(float time)
    {
        return shakeMagnitude * shakeOverLifetime.Evaluate(time);
    }

    public float GetFrequencyOverTime(float time)
    {
        return shakeFrequency * frequencyOverLifetime.Evaluate(time);
    }

    public AnimationCurve ShakeOverLifetime { get => shakeOverLifetime; private set => shakeOverLifetime = value; }
    public float LifeTime { get => lifeTime; private set => lifeTime = value; }
    public float ShakeMagnitude { get => shakeMagnitude; private set => shakeMagnitude = value; }
    public bool ShakeX { get => shakeX; private set => shakeX = value; }
    public bool ShakeY { get => shakeY; private set => shakeY = value; }
    public float ShakeFrequency { get => shakeFrequency; private set => shakeFrequency = value; }
    public bool ShakeZ { get => shakeZ; private set => shakeZ = value; }
    public AnimationCurve FrequencyOverLifetime { get => frequencyOverLifetime; set => frequencyOverLifetime = value; }
}