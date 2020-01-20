using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class LevelsProgressionWindow : EditorWindow
{
    List<LevelsScriptable> levelsToDisplay = new List<LevelsScriptable>();
    private string levelsPath = "Assets/ScriptableObjects/Levels";
    LevelsScriptable[] levels;
    LevelsScriptable currentLevel;

    public Texture2D texture;

    ReorderableList implementedLevels;

    Vector2 levelsScrollPos;
    Vector2 windowSpacePos;

    Vector2 boxPos;
    Vector2 boxSize;

    Vector2 buttonSize = new Vector2(50, 50);
    bool isHoldingLevel;

    bool isCheckForPosition;
    bool isLeftClick;

    bool isInArea;

    bool showLevels = true;
    string status = "Levels in Campaign";

    private GUIStyle labelStyle;
    private GUIStyle selectedStyle;

    string t = "This is a string inside a Scroll view!";





    [MenuItem("Window/Custom/Level Selection")]
    public static void OpenProgressionWindow()
    {
        LevelsProgressionWindow window = EditorWindow.GetWindow(typeof(LevelsProgressionWindow)) as LevelsProgressionWindow;

        window.Show();
    }

    private void OnEnable()
    {
        GetAllLevels();
        InitLevelProgression();
        InitStyles();
    }



    void OnGUI()
    {
        //Debug.Log("Mouse Position is " + Event.current.mousePosition);

        EventHandler();

        if (levelsToDisplay.Count > 0)
        {
            GraphicGUI();
        }

        ProgressionSettingGUI();

        DropAreaGUI();



        Repaint();
    }

    void InitStyles()
    {
        //Label Style
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 13;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.black;

        //Selected Style
        selectedStyle = new GUIStyle();
        selectedStyle.fontSize = 13;
        selectedStyle.fontStyle = FontStyle.Bold;
        selectedStyle.normal.textColor = Color.white;
    }

    void InitLevelProgression()
    {
        levelsToDisplay.Clear();

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].level.levelProgression.isImplemented)
            {
                levelsToDisplay.Add(levels[i]);
            }
        }
    }

    void ProgressionSettingGUI()
    {
        Handles.BeginGUI();

        boxSize = new Vector2(275, position.height - 20);
        boxPos = new Vector2(position.width - boxSize.x - 10, position.height - boxSize.y - 10);

        windowSpacePos = GUI.BeginScrollView(new Rect(new Vector2(0, 0), new Vector2(50000, 50000)), levelsScrollPos,
            new Rect(new Vector2(0, 0), new Vector2(100, 100)), true, true);

        BoxLevelsGUI();

        GUI.Box(new Rect(boxPos, boxSize), " ");


        if (currentLevel != null)
        {
            //Remplir les valeurs à changer
            currentLevel.level.levelProgression.isUnlocked = GUI.Toggle(new Rect(new Vector2(position.width - boxSize.x + 5, position.height - boxSize.y + 10), new Vector2(275, 15)), currentLevel.level.levelProgression.isUnlocked, "   Is this Level already Unlocked ? ");
        }
        else
        {
            EditorGUILayout.HelpBox("Aucun level sélectionné", MessageType.Info);
        }

        GUI.EndScrollView();

        Handles.EndGUI();
    }

    void GraphicGUI()
    {
        if (currentLevel != null)
        {
            MoveLevelOtpion();
        }

        for (int i = 0; i < levelsToDisplay.Count; i++)
        {
            DrawLevel(levelsToDisplay[i]);

            if (levelsToDisplay[i].level.levelProgression.unlockConditions.Count != 0)
                DrawConnections(levelsToDisplay[i]);
        }
    }

    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = new Rect(5, 5, position.width - 10, position.height - 10);
        GUI.Box(drop_area, "");

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

                            if (!CheckLevel((LevelsScriptable)dragged_object))
                            {
                                ImplementLevel((LevelsScriptable)dragged_object);
                            }
                        }
                    }
                }

                isInArea = false;
                break;
        }
    }


    bool CheckLevel(LevelsScriptable levelToCheck)
    {
        bool isAlreadyHere = false;

        for (int i = 0; i < levelsToDisplay.Count; i++)
        {
            if (levelsToDisplay[i] == levelToCheck)
            {
                Debug.Log("Already in");
                isAlreadyHere = true;
            }
        }

        return isAlreadyHere;
    }

    bool CheckConditions(LevelsScriptable conditionToCheck)
    {
        bool isAlreadyInCondition = false;

        for (int i = 0; i < currentLevel.level.levelProgression.unlockConditions.Count; i++)
        {
            if (currentLevel.level.levelProgression.unlockConditions[i] == conditionToCheck)
            {
                isAlreadyInCondition = true;
            }
        }

        return isAlreadyInCondition;
    }

    void ImplementLevel(LevelsScriptable level)
    {
        Debug.Log("Level Added");
        levelsToDisplay.Add(level);
        level.level.levelProgression.levelPos = new Vector2(position.width / 2, position.height / 2);
        level.level.levelProgression.isImplemented = true;
    }

    void EventHandler()
    {
        Event e = Event.current;

        if (Event.current.button == 0 && e.type == EventType.MouseDown)
        {
            isLeftClick = true;
        }

        if (Event.current.button == 0 && e.type == EventType.MouseUp)
        {
            isLeftClick = false;
        }

    }

    void MoveLevelOtpion()
    {
        Rect posButtonRect = new Rect(new Vector2(currentLevel.level.levelProgression.levelPos.x - 5, currentLevel.level.levelProgression.levelPos.y - 5),
            new Vector2(buttonSize.x + 10, buttonSize.y + 10));

        if (isCheckForPosition)
        {
            EditorGUI.DrawRect(posButtonRect, Color.Lerp(Color.white, Color.green, 0.3f));
        }
        else
        {
            EditorGUI.DrawRect(posButtonRect, Color.gray);
        }
    }

    void DrawLevel(LevelsScriptable levelConcerned)
    {
        Handles.BeginGUI();

        Vector2 levelPos = levelConcerned.level.levelProgression.levelPos;
        Rect levelRect = new Rect(levelPos, buttonSize);

        if (isCheckForPosition && isLeftClick)
        {
            UpdateLevelPosition();
        }


        if (GUI.Button(levelRect, levelConcerned.name))
        {
            if (Event.current.button == 0)
            {
                if (currentLevel != levelConcerned)
                {
                    currentLevel = levelConcerned;
                    Debug.Log("Level Selected : " + levelConcerned);
                }
            }

            if (Event.current.button == 1)
            {
                if (currentLevel != levelConcerned)
                    AddLevelAsConditionToCurrent(levelConcerned);
                else
                    isCheckForPosition = !isCheckForPosition;
            }

        }

        Handles.EndGUI();
    }

    void DrawConnections(LevelsScriptable level)
    {
        for (int i = 0; i < level.level.levelProgression.unlockConditions.Count; i++)
        {
            Vector2 levelselectedPos = level.level.levelProgression.levelPos;
            Vector2 conditionLevelPos = level.level.levelProgression.unlockConditions[i].level.levelProgression.levelPos;

            PositionChecker(conditionLevelPos, levelselectedPos);
        }
    }

    void GetAllLevels()
    {
        if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Levels"))
        {
            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { levelsPath });
            levels = new LevelsScriptable[levelsPaths.Length];

            for (int i = 0; i < levelsPaths.Length; i++)
            {

                levels[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelsPaths[i]), typeof(LevelsScriptable)) as LevelsScriptable;

            }
        }
        else
        {
            levels = new LevelsScriptable[0];
        }
    }


    void BoxLevelsGUI()
    {
        float space = 20f;

        //GUIStyle verticalScrollbar = GUI.skin.verticalScrollbar;
        //GUI.skin.verticalScrollbar = GUIStyle.none;

        //GUILayout.BeginVertical();

        levelsScrollPos = GUI.BeginScrollView(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 95), new Vector2(256, boxSize.y - 115)), levelsScrollPos,
            new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 95), new Vector2(256, space * levelsToDisplay.Count)), false, true/*, verticalScrollbar, verticalScrollbar*/);



        GUI.Box(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 95), new Vector2(256, boxSize.y)), " ");

        for (int i = 0; i < levelsToDisplay.Count; i++)
        {
            if (GUI.Button(new Rect(new Vector2(position.width - boxSize.x + 3, position.height - boxSize.y + 100 + (space * i)), new Vector2(250, 20)), ""))
            {
                currentLevel = levelsToDisplay[i];
            }

            if (levelsToDisplay[i] == currentLevel)
            {
                EditorGUI.DrawRect(new Rect(new Vector2(position.width - boxSize.x + 3, position.height - boxSize.y + 100 + (space * i)), new Vector2(250, 20)), Color.Lerp(Color.white, Color.black, 0.75f));
                GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 7, position.height - boxSize.y + 100 + (space * i)), new Vector2(250, 20)), levelsToDisplay[i].name, selectedStyle);
            }
            else
            {
                EditorGUI.DrawRect(new Rect(new Vector2(position.width - boxSize.x + 3, position.height - boxSize.y + 100 + (space * i)), new Vector2(250, 20)), Color.grey);
                GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 7, position.height - boxSize.y + 100 + (space * i)), new Vector2(250, 20)), levelsToDisplay[i].name, labelStyle);
            }
        }

        //GUILayout.EndVertical();

        GUI.EndScrollView();
    }

    void PositionChecker(Vector2 condition, Vector2 level)
    {
        Vector2 startLine = new Vector2(0, 0);
        Vector2 endLine = new Vector2(0, 0);

        float xStart = 0;
        float yStart = 0;
        float xStartOff = 0;
        float yStartOff = 0;

        float xEnd = 0;
        float yEnd = 0;
        float xEndOff = 0;
        float yEndOff = 0;


        //Start Line of Condition
        if ((condition.x + buttonSize.x) < level.x)
        {
            //Debug.Log("condition.x : " + condition.x);
            //Debug.Log("buttonSize.x : " + buttonSize.x);
            //Debug.Log("level.x : " + level.x);

            //if ((level.x - condition.x + buttonSize.x) > buttonSize.x)
            //{
            //    yStartOff = buttonSize.y * (level.x - condition.x + buttonSize.x / buttonSize.x);
            //}

            //Default values
            xStart = condition.x + buttonSize.x;
            yStart = condition.y + buttonSize.y / 2;

            xEnd = level.x;
            yEnd = level.y + buttonSize.y / 2;

            if (condition.y + buttonSize.y < level.y)
            {
                yStart = condition.y + buttonSize.y;
                yEnd = level.y;
            }

            if (condition.y > level.y + buttonSize.y)
            {
                yStart = condition.y;
                yEnd = level.y + buttonSize.y;
            }


        }

        if (condition.x > level.x + buttonSize.x)
        {
            //if ((level.x - condition.x + buttonSize.x) > buttonSize.x)
            //{
            //    yStartOff = buttonSize.y * (level.x - condition.x + buttonSize.x / buttonSize.x);
            //}

            //Default values
            xStart = condition.x;
            yStart = condition.y + buttonSize.y / 2;

            xEnd = level.x + buttonSize.x;
            yEnd = level.y + buttonSize.y / 2;

            if (condition.y + buttonSize.y < level.y)
            {
                yStart = condition.y + buttonSize.y;
                yEnd = level.y;
            }

            if (condition.y > level.y + buttonSize.y)
            {
                yStart = condition.y;
                yEnd = level.y + buttonSize.y;
            }


        }

        if (condition.x < level.x + buttonSize.x && condition.x + buttonSize.x > level.x)
        {
            xStart = condition.x + buttonSize.x / 2;
            yStart = condition.y + buttonSize.y;

            xEnd = level.x + buttonSize.x / 2;
            yEnd = level.y;

            if (condition.y > level.y + buttonSize.y)
            {
                yStart = condition.y;
                yEnd = level.y + buttonSize.y;
            }


            if (condition.x + buttonSize.x < level.x)
            {
                xStart = condition.x + buttonSize.x;
                xEnd = level.x;
            }

            if (condition.x > level.x + buttonSize.x)
            {
                xStart = condition.x;
                xEnd = level.x + buttonSize.x;
            }
        }

        startLine = new Vector2(xStart, yStart);
        endLine = new Vector2(xEnd, yEnd);


        Handles.DrawBezier(startLine, endLine, startLine, endLine, Color.white, null, 4f);

        Vector2 invDirection = (endLine - startLine).normalized;
        //Debug.Log(invDirection);

        Handles.ConeHandleCap(0, endLine, Quaternion.identity, 10, EventType.Repaint);
    }

    void DrawFramelGUI()
    {
        Handles.DrawSelectionFrame(1, new Vector2(currentLevel.level.levelProgression.levelPos.x + buttonSize.x / 2 + 1, currentLevel.level.levelProgression.levelPos.y + buttonSize.y / 2 + 1), Quaternion.LookRotation(Vector3.forward), 25, EventType.Repaint);
    }

    private void UpdateLevelPosition()
    {
        currentLevel.level.levelProgression.levelPos = new Vector2(Event.current.mousePosition.x - buttonSize.x / 2, Event.current.mousePosition.y - buttonSize.y / 2);
    }

    void AddLevelAsConditionToCurrent(LevelsScriptable condition)
    {
        if (!CheckConditions(condition))
        {
            currentLevel.level.levelProgression.unlockConditions.Add(condition);
        }
        else
        {
            currentLevel.level.levelProgression.unlockConditions.Remove(condition);
        }
    }

    void RemoveLevel(LevelsScriptable levelToRemove)
    {
        if (currentLevel != null)
        {
            for (int i = 0; i < levelsToDisplay.Count; i++)
            {
                if (levelsToDisplay[i].level.levelProgression.unlockConditions.Count > 0)
                    levelsToDisplay[i].level.levelProgression.unlockConditions.Remove(levelToRemove);
            }

            levelsToDisplay.Remove(levelToRemove);
        }
    }
}
