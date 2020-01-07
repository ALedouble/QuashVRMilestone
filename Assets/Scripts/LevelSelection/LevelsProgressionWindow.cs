using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class LevelsProgressionWindow : EditorWindow
{
    Texture2D levelTex;
    LevelsScriptable[] levelsToDisplay;

    Vector2 scrollPos;

    Vector2 boxPos;
    Vector2 boxSize;

    bool isInArea;

    [MenuItem("Window/Custom/Level Selection")]
    public static void OpenProgressionWindow()
    {
        LevelsProgressionWindow window = EditorWindow.GetWindow(typeof(LevelsProgressionWindow)) as LevelsProgressionWindow;

        window.Show();
    }

    void OnGUI()
    {
        ProgressionSettingGUI();

        DropAreaGUI();

        GraphicGUI();

        Repaint();
    }

    public void InitLevelProgression()
    {

    }

    void GraphicGUI()
    {
        Handles.BeginGUI();


        Handles.color = Handles.xAxisColor;

        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

        boxSize = new Vector2(275, position.height - 20);
        boxPos = new Vector2(position.width - boxSize.x - 10, position.height - boxSize.y - 10);

        GUI.Box(new Rect(boxPos, boxSize), "");

        EditorGUILayout.EndScrollView();

        //Handles.DrawLine(new Vector3(position.xMin, 0), new Vector3(position.xMax, position.height));


        Handles.EndGUI();
    }

    void ProgressionSettingGUI()
    {

    }

    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = new Rect(5, 5, position.width - 10, position.height - 10); /*GUILayoutUtility.GetRect(0.0f, 100, GUILayout.ExpandWidth(true));*/
        GUI.Box(drop_area, "Add Trigger");

        switch (evt.type)
        {
            case EventType.DragExited:
                isInArea = false;
                break;

            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                Debug.Log("inArea");
                isInArea = true;

                if (evt.type == EventType.DragPerform)
                {
                    Debug.Log("Dragged");

                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged_object in DragAndDrop.objectReferences)
                    {
                        if (dragged_object is LevelsScriptable)
                        {
                            Debug.Log("Drag accepted");

                        }
                    }
                }

                isInArea = false;
                break;
        }
    }
}
