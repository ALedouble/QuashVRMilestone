using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BrickManager))]
public class BrickManagerInspector : Editor
{
    BrickManager myTarget;


#if (UNITY_EDITOR)
    private void OnEnable()
    {
        myTarget = (BrickManager)target;

        InitPrefab();
        InitBrickPresets();
        InitColorPresets();
    }
#endif


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }


    private void InitPrefab()
    {
        if (AssetDatabase.IsValidFolder(myTarget.prefabPath))
        {
            string[] prefabPaths = AssetDatabase.FindAssets("t:gameobject", new string[] { myTarget.prefabPath });

            myTarget.prefabBase = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabPaths[0]), typeof(GameObject)) as GameObject;
        }
        else
        {
            myTarget.prefabBase = null;
        }
    }

    private void InitBrickPresets()
    {
        if (AssetDatabase.IsValidFolder(myTarget.presetPath))
        {
            string[] presetsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { myTarget.brickPresetPath });
            myTarget.brickPresets = new BrickTypesScriptable[presetsPaths.Length];

            for (int i = 0; i < presetsPaths.Length; i++)
            {
                myTarget.brickPresets[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(presetsPaths[i]), typeof(BrickTypesScriptable)) as BrickTypesScriptable;
            }
        }
        else
        {
            myTarget.brickPresets = new BrickTypesScriptable[0];
        }
    }

    private void InitColorPresets()
    {
        if (AssetDatabase.IsValidFolder(myTarget.presetPath))
        {
            string[] presetsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { myTarget.presetPath });
            myTarget.colorPresets = new PresetScriptable[presetsPaths.Length];

            for (int i = 0; i < presetsPaths.Length; i++)
            {

                myTarget.colorPresets[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(presetsPaths[i]), typeof(PresetScriptable)) as PresetScriptable;

            }
        }
        else
        {
            myTarget.colorPresets = new PresetScriptable[0];
        }
    }
}
