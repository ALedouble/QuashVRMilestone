using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "SC_EditorPreset_", menuName = "Custom/Editor Preset", order = 122)]
public class EditorScriptable : ScriptableObject
{
    public List<Vector3> editorSpaceRecorded;

    public int rows;
    public int columns;
    public float cellSize;
}
