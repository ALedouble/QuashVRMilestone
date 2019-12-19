using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[CreateAssetMenu(fileName = "SC_ColorPreset_", menuName = "Custom/ColorPreset", order = 130)]
public class PresetScriptable : ScriptableObject
{
    public ColorPreset[] colorPresets;
}

[System.Serializable]
public struct ColorPreset
{
    public string tag;
    [ColorUsage(true, true)] public Color fresnelColors;
    [ColorUsage(true, true)] public Color coreEmissiveColors;
}
