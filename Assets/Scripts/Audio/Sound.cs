using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sound
{
    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    public float defaultVolume;

    [Range(0.1f, 3.0f)]
    public float pitch;

    public bool loop;

    [Range(0.0f, 1.0f)]
    public float spatialBlend;

    [Range(-1.0f, 1.0f)]
    public float panStereo;

    [Range(0.0f, 1.0f)]
    public float minVolume;

    [Range(0.0f, 1.0f)]
    public float maxVolume;
}
