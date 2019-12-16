using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SC_BrickPreset_", menuName = "Custom/BrickPreset", order = 135)]
public class BrickTypesScriptable : ScriptableObject
{
    public BrickPreset[] brickPresets;
}

[System.Serializable]
public struct BrickPreset
{
    public string tag;
    public int armorValue;
    public int scoreValue;
}
