using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Malee.Editor;


<<<<<<< HEAD
//[CustomEditor(typeof(SoundPreset))]
//public class SoundPresetInspector : Editor
//{
//    Malee.Editor.ReorderableList soundPoolList;
//    SerializedProperty soundPoolProperty;


//    private void OnEnable()
//    {
//        soundPoolProperty = serializedObject.FindProperty("soundPools");

//        soundPoolList = new Malee.Editor.ReorderableList(soundPoolProperty);
//        //audioList.surrogate = new Malee.Editor.ReorderableList.Surrogate(typeof(GameObject), AppendObject);
//    }

//    //void AppendObject(SerializedProperty element, UnityEngine.Object objectReference, Malee.Editor.ReorderableList list)
//    //{
//    //    element.FindPropertyRelative("name").stringValue = objectReference.name;
//    //    element.FindPropertyRelative("clip").objectReferenceValue = objectReference;
//    //}


//    public override void OnInspectorGUI()
//    {
//        EditorGUI.BeginChangeCheck();

//        serializedObject.Update();


//        GUILayout.Space(8);
//        soundPoolList.DoLayoutList();

//        GUILayout.Space(50);

//        serializedObject.ApplyModifiedProperties();

//        if (EditorGUI.EndChangeCheck())
//        {

//        }
//    }
//}
=======
[CustomEditor(typeof(SoundPreset))]
public class SoundPresetInspector : Editor
{
    Malee.Editor.ReorderableList soundPoolList;
    SerializedProperty soundPool;


    private void OnEnable()
    {
        soundPool = serializedObject.FindProperty("soundPools");

        soundPoolList = new Malee.Editor.ReorderableList(soundPool);
        //audioList.surrogate = new Malee.Editor.ReorderableList.Surrogate(typeof(GameObject), AppendObject);
    }

    //void AppendObject(SerializedProperty element, UnityEngine.Object objectReference, Malee.Editor.ReorderableList list)
    //{
    //    element.FindPropertyRelative("name").stringValue = objectReference.name;
    //    element.FindPropertyRelative("clip").objectReferenceValue = objectReference;
    //}


    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();


        GUILayout.Space(8);
        soundPoolList.DoLayoutList();

        GUILayout.Space(50);

        serializedObject.ApplyModifiedProperties();

        EditorGUI.EndChangeCheck();
    }
}
>>>>>>> TimoPiau
