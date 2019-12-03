using UnityEngine;
using UnityEditor;
using Malee.Editor;

[CustomEditor(typeof(EditorScriptable))]
public class EditorPresetInspector : Editor
{
    ReorderableList myPresetList;
    SerializedProperty presetsArray;
    SerializedProperty cellSizeProp;
    SerializedProperty columnsProp;
    SerializedProperty rowsProp;



    private void OnEnable()
    {
        presetsArray = serializedObject.FindProperty("editorSpaceRecorded");
        cellSizeProp = serializedObject.FindProperty("cellSize");
        columnsProp = serializedObject.FindProperty("columns");
        rowsProp = serializedObject.FindProperty("rows");

        myPresetList = new ReorderableList(presetsArray, false, false, false);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        serializedObject.Update();


        GUILayout.Space(8);
        myPresetList.DoLayoutList();

        EditorGUILayout.PropertyField(cellSizeProp);
        EditorGUILayout.PropertyField(columnsProp);
        EditorGUILayout.PropertyField(rowsProp);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {

        }
    }
}
