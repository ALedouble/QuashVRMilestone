using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundPreset.asset", menuName = "Custom/SoundPreset", order = 500)]
public class SoundPreset : ScriptableObject
{
    public SoundPool[] soundPools;
}
