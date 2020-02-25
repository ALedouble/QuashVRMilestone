using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class LevelsProgressionWindow : EditorWindow
{
    List<LevelsScriptable> levelsToDisplay = new List<LevelsScriptable>();
    private string levelsPath = "Assets/ScriptableObjects/Levels/FinalLevels";
    LevelsScriptable[] levels;
    LevelsScriptable currentLevel;

    public Texture2D texture;

    ReorderableList implementedLevels;

    Vector2 levelsScrollPos;
    Vector2 windowSpacePos;

    Vector2 windowSize = new Vector2(300, 1000);

    Vector2 boxPos;
    Vector2 boxSize;

    Vector2 treeDelimitation = new Vector2(300, 1000);

    Vector2 buttonSize = new Vector2(50, 50);
    bool isHoldingLevel;

    bool isCheckForPosition;
    bool isLeftClick;
    bool isRightClick;
    private bool isControlDown;

    private bool isInArea;

    private GUIStyle labelStyle;
    private GUIStyle selectedStyle;
    private GUIStyle dropStyle;

    float configBottom;
    private bool mouseOutOfWindow;



    [MenuItem("Window/Campaign Editor")]
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

        treeDelimitation = new Vector2(600, 2000);
        windowSize = new Vector2(600, 2000);
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

        //Drop Style
        dropStyle = new GUIStyle();
        dropStyle.fontSize = 18;
        dropStyle.fontStyle = FontStyle.Normal;
        dropStyle.normal.textColor = Color.white;
        dropStyle.alignment = TextAnchor.UpperCenter;
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
        //Handles.BeginGUI();

        boxSize = new Vector2(275, position.height - 20);
        boxPos = new Vector2(position.width - boxSize.x - 10, position.height - boxSize.y - 10);

        BoxLevelsGUI();

        GUI.Box(new Rect(boxPos, boxSize), " ");

        if (currentLevel != null)
        {
            EditorGUI.BeginChangeCheck();

            GUI.color = Color.red;
            if (GUI.Button(new Rect(new Vector2(position.width - 85, position.height - boxSize.y - 3f), new Vector2(69, 20)), ""))
            {
                RemoveLevel(currentLevel);
                return;
            }
            GUI.color = Color.white;

            GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 5, position.height - boxSize.y), new Vector2(245, 15)), "- " + currentLevel.name + " - " + currentLevel.level.levelProgression.buttonName, selectedStyle);

            GUI.Label(new Rect(new Vector2(position.width - 80, position.height - boxSize.y), new Vector2(69, 20)), "Remove", selectedStyle);


            //Text 4 the button
            GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 50, position.height - boxSize.y + 18), new Vector2(120, 15)), " Button Text -");

            currentLevel.level.levelProgression.buttonName = GUI.TextField(new Rect(new Vector2(position.width - boxSize.x + 5, position.height - boxSize.y + 18), new Vector2(40, 15)),
                currentLevel.level.levelProgression.buttonName);

            //Is the level unlocked ?
            currentLevel.level.levelProgression.isUnlocked = GUI.Toggle(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 45), new Vector2(90, 15)),
                currentLevel.level.levelProgression.isUnlocked, " Unlocked ? ");

            //Conditions needed to be unlocked
            GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 120, position.height - boxSize.y + 45), new Vector2(100, 15)), "Required STARS");

            currentLevel.level.levelProgression.starsRequired = EditorGUI.IntField(new Rect(new Vector2(position.width - boxSize.x + 220, position.height - boxSize.y + 45), new Vector2(38, 15)),
                currentLevel.level.levelProgression.starsRequired);


            //1nd condition
            currentLevel.level.levelProgression.conditionsToComplete[0].conditionComparator =
                (CompleteConditionComparator)EditorGUI.EnumPopup(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 65), new Vector2(50, 15)),
                currentLevel.level.levelProgression.conditionsToComplete[0].conditionComparator);
            currentLevel.level.levelProgression.conditionsToComplete[0].conditionType =
                (CompleteConditionType)EditorGUI.EnumPopup(new Rect(new Vector2(position.width - boxSize.x + 50, position.height - boxSize.y + 65), new Vector2(200, 15)),
                currentLevel.level.levelProgression.conditionsToComplete[0].conditionType);

            currentLevel.level.levelProgression.conditionsToComplete[0].conditionReachedAt =
                EditorGUI.IntField(new Rect(new Vector2(position.width - boxSize.x + 5, position.height - boxSize.y + 80), new Vector2(250, 15)),
                currentLevel.level.levelProgression.conditionsToComplete[0].conditionReachedAt);

            //2nd condition
            currentLevel.level.levelProgression.conditionsToComplete[1].conditionComparator =
                (CompleteConditionComparator)EditorGUI.EnumPopup(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 100), new Vector2(50, 15)),
                currentLevel.level.levelProgression.conditionsToComplete[1].conditionComparator);
            currentLevel.level.levelProgression.conditionsToComplete[1].conditionType =
                (CompleteConditionType)EditorGUI.EnumPopup(new Rect(new Vector2(position.width - boxSize.x + 50, position.height - boxSize.y + 100), new Vector2(200, 15)),
                currentLevel.level.levelProgression.conditionsToComplete[1].conditionType);

            currentLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt =
                EditorGUI.IntField(new Rect(new Vector2(position.width - boxSize.x + 5, position.height - boxSize.y + 115), new Vector2(250, 15)),
                currentLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt);


            ///Level Specifics (exotic rules)
            currentLevel.level.levelSpec.noWallsMode = GUI.Toggle(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 135), new Vector2(110, 15)),
                currentLevel.level.levelSpec.noWallsMode, " No Walls Mode");

            currentLevel.level.levelSpec.mandatoryBounce = GUI.Toggle(new Rect(new Vector2(position.width - boxSize.x + 160, position.height - boxSize.y + 135), new Vector2(100, 15)),
                            currentLevel.level.levelSpec.mandatoryBounce, " Bounce Mode");

            currentLevel.level.levelSpec.changingBrickColorEveryXseconds = GUI.Toggle(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 155), new Vector2(170, 15)),
                currentLevel.level.levelSpec.changingBrickColorEveryXseconds, " Brick Color Change Mode");


            GUI.Label(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + 175), new Vector2(130, 15)), "Switch Behaviour");

            configBottom = 135;

            currentLevel.level.levelSpec.switchColorBehaviourForThisLevel =
                (ColorSwitchBehaviour)EditorGUI.EnumPopup(new Rect(new Vector2(position.width - boxSize.x + 125, position.height - boxSize.y + 175), new Vector2(130, 15)),
                currentLevel.level.levelSpec.switchColorBehaviourForThisLevel);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(currentLevel);
            }
        }
        else
        {
            GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 5, position.height - boxSize.y), new Vector2(245, 15)), "Aucun Level Selectionné", labelStyle);
        }

        //Handles.EndGUI();



    }

    void GraphicGUI()
    {
        GUI.color = Color.black;
        GUI.Box(new Rect(new Vector2(5, 5), new Vector2((treeDelimitation.x + (buttonSize.x) - 5), treeDelimitation.y - 5)), " ");
        GUI.color = Color.white;

        //Level layout space view
        windowSpacePos = GUI.BeginScrollView(new Rect(new Vector2(0, 0), new Vector2(position.width - boxSize.x - 10, position.height)), windowSpacePos,
            new Rect(new Vector2(0, 0), new Vector2(windowSize.x + 40, windowSize.y)), true, false);

        if (currentLevel != null)
        {
            MoveLevelOtpion();
        }

        for (int i = 0; i < levelsToDisplay.Count; i++)
        {
            DrawLevel(levelsToDisplay[i]);

            if (levelsToDisplay[i].level.levelProgression.unlockConditions.Count > 0)
            {
                DrawConnections(levelsToDisplay[i]);
            }
        }

        GUI.EndScrollView();
    }

    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = new Rect(0, 0, position.width, position.height);
        GUI.Box(drop_area, "");

        if (levelsToDisplay.Count == 0)
            GUI.Label(new Rect(new Vector2(((position.width - boxSize.x) * 0.5f - 105), (position.height * 0.5f)), new Vector2(200f, 15f)), "Drag your LEVEL(s) here", dropStyle);

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

                isInArea = true;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged_object in DragAndDrop.objectReferences)
                    {
                        if (dragged_object is LevelsScriptable)
                        {
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
                continue;
            }
        }

        return isAlreadyInCondition;
    }

    void ImplementLevel(LevelsScriptable level)
    {
        //Debug.Log("Level Added");
        levelsToDisplay.Add(level);
        level.level.levelProgression.levelPos = new Vector2(position.width / 2, position.height / 2);
        level.level.levelProgression.isImplemented = true;

        EditorUtility.SetDirty(level);

        InitLevelProgression();
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

        if (Event.current.button == 1 && e.type == EventType.MouseDown)
        {
            isRightClick = true;
        }

        if (Event.current.button == 1 && e.type == EventType.MouseUp)
        {
            isRightClick = false;
        }

        if (Event.current.control)
        {
            isControlDown = true;
        }
        else
        {
            isControlDown = false;
        }

        if (Event.current.mousePosition.x < 0 || Event.current.mousePosition.y < 0 || Event.current.mousePosition.x > position.width || Event.current.mousePosition.y > position.height)
        {
            mouseOutOfWindow = true;
        }
        else
        {
            mouseOutOfWindow = false;
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

        if (isRightClick && !mouseOutOfWindow)
        {
            UpdateLevelPosition();
            isCheckForPosition = true;
        }
        else
        {
            isCheckForPosition = false;
        }


        if (GUI.Button(levelRect, levelConcerned.level.levelProgression.buttonName))
        {
            if (Event.current.button == 0 && !isControlDown)
            {
                if (currentLevel != levelConcerned)
                {
                    currentLevel = levelConcerned;
                    //Debug.Log("Level Selected : " + levelConcerned);
                }
            }

            if (Event.current.button == 0 && isControlDown)
            {
                if (currentLevel != levelConcerned)
                    AddLevelAsConditionToCurrent(levelConcerned);
                //else
                //    isCheckForPosition = !isCheckForPosition;
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

        if (levelsToDisplay.Count > 0)
        {
            levelsScrollPos = GUI.BeginScrollView(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + configBottom + 3 + 60), new Vector2(256, boxSize.y - 145 - 60)), levelsScrollPos,
            new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + configBottom + 3), new Vector2(256, space * levelsToDisplay.Count)), false, false);

            GUI.Box(new Rect(new Vector2(position.width - boxSize.x, position.height - boxSize.y + configBottom + 3), new Vector2(256, boxSize.y)), " ");

            for (int i = 0; i < levelsToDisplay.Count; i++)
            {
                if (GUI.Button(new Rect(new Vector2(position.width - boxSize.x + 3, position.height - boxSize.y + configBottom + 7 + (space * i)), new Vector2(250, 20)), ""))
                {
                    currentLevel = levelsToDisplay[i];
                    //GUI.ScrollTo(new Rect(levelsToDisplay[i].level.levelProgression.levelPos, new Vector2(windowSize.x, treeDelimitation.y)));
                }

                if (levelsToDisplay[i] == currentLevel)
                {
                    EditorGUI.DrawRect(new Rect(new Vector2(position.width - boxSize.x + 3, position.height - boxSize.y + configBottom + 7 + (space * i)), new Vector2(250, 20)), Color.Lerp(Color.white, Color.black, 0.75f));
                    GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 7, position.height - boxSize.y + configBottom + 7 + (space * i)), new Vector2(250, 20)),
                        levelsToDisplay[i].name + "     - " + levelsToDisplay[i].level.levelProgression.buttonName, selectedStyle);
                }
                else
                {
                    EditorGUI.DrawRect(new Rect(new Vector2(position.width - boxSize.x + 3, position.height - boxSize.y + configBottom + 7 + (space * i)), new Vector2(250, 20)), Color.grey);
                    GUI.Label(new Rect(new Vector2(position.width - boxSize.x + 7, position.height - boxSize.y + configBottom + 7 + (space * i)), new Vector2(250, 20)),
                        levelsToDisplay[i].name + "     - " + levelsToDisplay[i].level.levelProgression.buttonName, labelStyle);
                }
            }

            GUI.EndScrollView();
        }
    }

    void PositionChecker(Vector2 condition, Vector2 level)
    {
        Vector2 startLine = new Vector2(0, 0);
        Vector2 endLine = new Vector2(0, 0);

        float xStart = 0;
        float yStart = 0;
        //float xStartOff = 0;
        //float yStartOff = 0;

        float xEnd = 0;
        float yEnd = 0;
        //float xEndOff = 0;
        //float yEndOff = 0;


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

        Handles.BeginGUI();

        Handles.DrawBezier(startLine, endLine, startLine, endLine, Color.white, null, 4f);

        Vector3 invDirection = Vector3.Normalize(endLine - startLine);


        Vector3 product = Vector3.Cross(invDirection, Vector3.forward);

        Vector3 arrow01 = invDirection + (new Vector3(startLine.x, startLine.y, 0) * 20);
        Vector3 arrow02 = invDirection + (new Vector3(startLine.x, startLine.y, 0) * 20);

        Handles.ConeHandleCap(0, endLine, Quaternion.identity, 10, EventType.Repaint);

        Handles.EndGUI();
    }

    void DrawFramelGUI()
    {
        Handles.DrawSelectionFrame(1, new Vector2(currentLevel.level.levelProgression.levelPos.x + buttonSize.x / 2 + 1, currentLevel.level.levelProgression.levelPos.y + buttonSize.y / 2 + 1), Quaternion.LookRotation(Vector3.forward), 25, EventType.Repaint);
    }

    private void UpdateLevelPosition()
    {
        //Check X position of the level
        if ((Event.current.mousePosition.x - buttonSize.x / 2) <= (windowSize.x) && (Event.current.mousePosition.x - buttonSize.x / 2) >= 0)
        {
            //Check Y position of the level
            if ((Event.current.mousePosition.y - buttonSize.y / 2) <= treeDelimitation.y && (Event.current.mousePosition.y - buttonSize.y / 2) >= 0)
                currentLevel.level.levelProgression.levelPos = new Vector2(Event.current.mousePosition.x - buttonSize.x / 2, Event.current.mousePosition.y - buttonSize.y / 2);
            else
                currentLevel.level.levelProgression.levelPos = new Vector2(Event.current.mousePosition.x - buttonSize.x / 2, currentLevel.level.levelProgression.levelPos.y);
        }
        else
        {
            //Check Y position of the level
            if ((Event.current.mousePosition.y - buttonSize.y / 2) <= treeDelimitation.y && (Event.current.mousePosition.y - buttonSize.y / 2) >= 0)
                currentLevel.level.levelProgression.levelPos = new Vector2(currentLevel.level.levelProgression.levelPos.x, Event.current.mousePosition.y - buttonSize.y / 2);
            else
                currentLevel.level.levelProgression.levelPos = new Vector2(currentLevel.level.levelProgression.levelPos.x, currentLevel.level.levelProgression.levelPos.y);
        }

        EditorUtility.SetDirty(currentLevel);
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

        EditorUtility.SetDirty(currentLevel);
    }

    void RemoveLevel(LevelsScriptable levelToRemove)
    {
        if (currentLevel != null)
        {
            for (int i = 0; i < levelsToDisplay.Count; i++)
            {
                if (levelsToDisplay[i] != levelToRemove)
                {
                    if (levelsToDisplay[i].level.levelProgression.unlockConditions.Count != 0)
                    {
                        if (levelsToDisplay[i].level.levelProgression.unlockConditions.Contains(levelToRemove))
                            levelsToDisplay[i].level.levelProgression.unlockConditions.Remove(levelToRemove);
                    }
                }
            }

            levelToRemove.level.levelProgression = new ProgressionSettings();
            levelsToDisplay.Remove(levelToRemove);

            for (int i = 0; i < levelsToDisplay.Count; i++)
            {
                Debug.Log(levelsToDisplay[i]);
            }

            currentLevel = null;
            RefreshInspector();
            InitLevelProgression();
        }

    }

    bool CheckLockCondition(LevelsScriptable levelToCheck)
    {
        int totalStars = 0;

        //Check si tous les niveaux reliés à ce niveau ont été finis avant
        for (int i = 0; i < levelToCheck.level.levelProgression.unlockConditions.Count; i++)
        {
            if (!levelToCheck.level.levelProgression.unlockConditions[i].level.levelProgression.isDone)
                return false;
            else
                totalStars += levelToCheck.level.levelProgression.unlockConditions[i].level.levelProgression.numberOfConditionCompleted;
        }

        //Et si le nombre d'étoile nécéssaire pour son déblocage a été atteint
        if (levelToCheck.level.levelProgression.starsRequired < totalStars)
            return false;
        else
            return true;
    }

    void RefreshInspector()
    {
        LevelsProgressionWindow[] cew = Resources.FindObjectsOfTypeAll(typeof(LevelsProgressionWindow)) as LevelsProgressionWindow[];
        if (cew.Length != 0)
        {
            for (int i = 0; i < cew.Length; i++)
            {
                cew[i].Repaint();
            }
        }
    }
}
