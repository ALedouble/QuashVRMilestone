using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LevelManager))]
public class LevelManagerInspector : Editor
{
    LevelManager mytarget;

#if (UNITY_EDITOR)
    private void OnEnable()
    {
        mytarget = (LevelManager)target;

        //if (CheckForLevels())
        //{
        //    GetAllLevels();
        //}
        //else
        //{
        //    mytarget.registeredLevels = new LevelsScriptable[0];
        //}
    }
#endif

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }




    void GetAllLevels()
    {
            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { mytarget.levelsPath });
            mytarget.registeredLevels = new LevelsScriptable[levelsPaths.Length];

            for (int i = 0; i < levelsPaths.Length; i++)
            {
                mytarget.registeredLevels[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelsPaths[i]), typeof(LevelsScriptable)) as LevelsScriptable;
            }
    }

    bool CheckForLevels()
    {
        return AssetDatabase.IsValidFolder(mytarget.levelsPath);
    }
}
