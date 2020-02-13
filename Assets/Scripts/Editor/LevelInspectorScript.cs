using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using TMPro;


[CustomEditor(typeof(LevelScript))]
public class LevelInspectorScript : Editor
{
    public enum EditionMode
    {
        Paint2D,
        Erase2D,
        Select,
        Waypoint,
    }

    private EditionMode selectedMode;
    private EditionMode currentMode;

    public enum PaintMode
    {
        OnDraw,
        OnSelection
    }

    PaintMode paintMode;

    private string[] paintModeLabel;
    BrickSettings paintedBrickSettings;

    BrickSettings brickSettingsDisplayed;
    int brickPosition;

    LevelScript myTarget;



    private int newTotalColumns;
    private int newTotalRows;
    private float newCellSize;

    private GUIStyle titleStyle;
    private GUIStyle layerStyle;
    private GUIStyle btnStyle;
    private GUIStyle slashStyle;
    private GUIStyle noneStyle;
    private GUIStyle toolStyle;
    private GUIStyle wayStyle;


    SerializedProperty editorSpaceProperty;
    ReorderableList mySpace;

    SerializedProperty paintedBrickSet;
    ReorderableList waypointStorageList;


    private EditorScriptable editorPreset;
    private string editorPath = "Assets/ScriptableObjects/EditorPreset";

    private string levelsPath = "Assets/ScriptableObjects/Levels";
    LevelsScriptable[] levels;
    LevelsScriptable currentLevel;

    private GameObject prefabBase;
    private string prefabPath = "Assets/Resources/Bricks";

    private GameObject waypointIcon;
    private string iconPath = "Assets/Prefabs/Editor/WaypointPrefab";

    private GameObject frontWall;
    private string frontPath = "Assets/Prefabs/Editor/FrontWallPrefab";

    private PresetScriptable[] colorPresets;
    private string presetPath = "Assets/ScriptableObjects/ColorPresets";

    private BrickTypesScriptable[] brickPresets;
    private string brickPresetPath = "Assets/ScriptableObjects/BrickPresets";

    List<GameObject> waypointsgo;


    WallBuilds walls;
    Wall currentLayer;
    int selectedLayer;
    int numberOfLayers;
    int totalLayersDisplayed;

    LevelSpecifics specifics;

    bool canPaintWaypoint;
    bool nameSaved = true;
    bool changingNumberOfLayers;

#if (UNITY_EDITOR)
    public void OnEnable()
    {
        myTarget = (LevelScript)target;
        waypointsgo = new List<GameObject>();



        Undo.undoRedoPerformed += MyUndoCallBack;



        InitEditor();
        InitWaypointPrefab();
        InitFrontPrefab();
        InitPrefab();
        InitBrickPresets();
        InitColorPresets();

        InitWReorderableList();
        InitGridValues();
        InitReorderableList();
        InitSelectedLevelValues();
        GetAllLevels();
        InitEditModes();
        InitStyles();
    }

    private void OnDisable()
    {
        CleanLayer();
    }
#endif



    void InitReorderableList()
    {
        paintedBrickSet = serializedObject.FindProperty("brickWaypoints");
        waypointStorageList = new ReorderableList(serializedObject, paintedBrickSet, true, true, true, true);

        waypointStorageList.drawHeaderCallback = MyListHeader;
        waypointStorageList.drawElementCallback = MyListElementDrawer;
        waypointStorageList.onAddCallback += MyListAddCallback;
        waypointStorageList.onRemoveCallback += MyListRemoveCallback;
        waypointStorageList.onReorderCallback += (ReorderableList list) => { DrawWaypointIcon(); };
    }

    #region Reorderlist Stuff

    void MyListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Waypoints");
    }

    void MyListElementDrawer(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.yMin += 2;
        rect.yMax -= 4;
        EditorGUI.PropertyField(rect, paintedBrickSet.GetArrayElementAtIndex(index), new GUIContent("Waypoint " + index.ToString()));
    }

    void MyListAddCallback(UnityEditorInternal.ReorderableList rlist)
    {
        paintedBrickSet.arraySize++;
        SerializedProperty sp = paintedBrickSet.GetArrayElementAtIndex(paintedBrickSet.arraySize - 1);
        sp.vector3Value = new Vector3(0, 0, 0);
    }

    void MyListRemoveCallback(UnityEditorInternal.ReorderableList rlist)
    {
        paintedBrickSet.DeleteArrayElementAtIndex(rlist.index);
    }
    #endregion

    void InitWReorderableList()
    {
        editorSpaceProperty = serializedObject.FindProperty("editorSpace");
        mySpace = new ReorderableList(serializedObject, editorSpaceProperty, false, false, false, false);

        mySpace.drawHeaderCallback = MyWListHeader;
        mySpace.drawElementCallback = MyWListElementDrawer;
        //mySpace.onReorderCallback += (ReorderableList list) =>
        //{
        //    Debug.Log("la liste vient d'être réordonnée");
        //};
    }

    #region Reorderlist Stuff

    void MyWListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Editor Space");
    }

    void MyWListElementDrawer(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.yMin += 2;
        rect.yMax -= 4;
        EditorGUI.PropertyField(rect, editorSpaceProperty.GetArrayElementAtIndex(index), new GUIContent("Ref " + index.ToString()));
    }
    #endregion




    private void InitPrefab()
    {
        if (AssetDatabase.IsValidFolder(prefabPath))
        {
            string[] prefabPaths = AssetDatabase.FindAssets("t:gameobject", new string[] { prefabPath });

            prefabBase = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabPaths[0]), typeof(GameObject)) as GameObject;

            myTarget.brickPrefab = prefabBase;
        }
        else
        {
            prefabPath = null;
            Debug.LogError("Prefab is missing");
        }
    }

    private void InitWaypointPrefab()
    {
        if (AssetDatabase.IsValidFolder(iconPath))
        {
            string[] prefabPaths = AssetDatabase.FindAssets("t:gameobject", new string[] { iconPath });

            waypointIcon = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabPaths[0]), typeof(GameObject)) as GameObject;
        }
        else
        {
            Debug.LogError("Waypoint Prefab is missing");
        }
    }

    private void InitFrontPrefab()
    {
        if (AssetDatabase.IsValidFolder(frontPath))
        {
            string[] prefabPaths = AssetDatabase.FindAssets("t:gameobject", new string[] { frontPath });

            frontWall = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabPaths[0]), typeof(GameObject)) as GameObject;
        }
        else
        {
            Debug.LogError("FrontWall Prefab is missing");
        }
    }

    private void InitEditor()
    {
        if (AssetDatabase.IsValidFolder(editorPath))
        {
            string[] editorPaths = AssetDatabase.FindAssets("t:scriptableobject", new string[] { editorPath });

            editorPreset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(editorPaths[0]), typeof(EditorScriptable)) as EditorScriptable;
        }
        else
        {
            editorPreset = null;

            Debug.LogError("Editor Preset is missing");
        }
    }

    private void InitBrickPresets()
    {
        if (AssetDatabase.IsValidFolder(presetPath))
        {
            string[] presetsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { brickPresetPath });
            brickPresets = new BrickTypesScriptable[presetsPaths.Length];

            for (int i = 0; i < presetsPaths.Length; i++)
            {

                brickPresets[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(presetsPaths[i]), typeof(BrickTypesScriptable)) as BrickTypesScriptable;

            }

            myTarget.brickPresets = brickPresets;
        }
        else
        {
            myTarget.brickPresets = new BrickTypesScriptable[0];
            brickPresets = new BrickTypesScriptable[0];

            Debug.LogError("Brick Preset is missing");
        }
    }

    private void InitColorPresets()
    {
        if (AssetDatabase.IsValidFolder(presetPath))
        {
            string[] presetsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { presetPath });
            colorPresets = new PresetScriptable[presetsPaths.Length];

            for (int i = 0; i < presetsPaths.Length; i++)
            {

                colorPresets[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(presetsPaths[i]), typeof(PresetScriptable)) as PresetScriptable;

            }

            myTarget.colorPresets = colorPresets;
        }
        else
        {
            myTarget.colorPresets = new PresetScriptable[0];
            colorPresets = new PresetScriptable[0];

            Debug.LogError("Color Preset is missing");
        }
    }

    public void InitGridValues()
    {
        myTarget.editorSpace = editorPreset.editorSpaceRecorded;

        myTarget.CellSize = editorPreset.cellSize;
        myTarget.totalColumns = editorPreset.columns;
        myTarget.totalRows = editorPreset.rows;

        newCellSize = myTarget.CellSize;
        newTotalColumns = myTarget.totalRows;
        newTotalRows = myTarget.totalColumns;
    }

    public void InitSelectedLevelValues()
    {
        if (myTarget.selectedLevel != null)
        {
            currentLevel = myTarget.selectedLevel;
            numberOfLayers = currentLevel.level.levelWallBuilds.walls.Length;
            totalLayersDisplayed = numberOfLayers - 1;
            selectedLayer = 0;
            myTarget.bricksOnLayer = 0;
            specifics = currentLevel.level.levelSpec;


            myTarget.bricksOnScreen = new GameObject[editorPreset.columns * editorPreset.rows];

            CleanLayer();
            SpawnLayer();
        }
    }

    public void InitStyles()
    {
        //Tittle Style
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 14;
        titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        //Layer Label Style
        layerStyle = new GUIStyle();
        layerStyle.fontStyle = FontStyle.Bold;
        layerStyle.alignment = TextAnchor.MiddleCenter;
        layerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        //Button Style
        btnStyle = new GUIStyle();
        btnStyle.alignment = TextAnchor.MiddleCenter;
        btnStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        btnStyle.fontStyle = FontStyle.Bold;
        btnStyle.fontSize = 15;

        //Slash Style
        slashStyle = new GUIStyle();
        slashStyle.alignment = TextAnchor.MiddleCenter;
        slashStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        //End of an Option
        noneStyle = new GUIStyle();
        noneStyle.alignment = TextAnchor.MiddleCenter;
        noneStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        //toolStyle Style
        toolStyle = new GUIStyle();
        toolStyle.alignment = TextAnchor.MiddleCenter;
        toolStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        toolStyle.fontSize = 10;

        //Waypoint Style
        wayStyle = new GUIStyle();
        wayStyle.alignment = TextAnchor.MiddleCenter;
        wayStyle.normal.textColor = Color.black;
        wayStyle.fontStyle = FontStyle.Bold;
        wayStyle.fontSize = 10;
    }

    private void InitEditModes()
    {
        paintModeLabel = new string[2] { "onDraw", "onSelection" };
    }

    private void GetAllLevels()
    {
        if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Levels"))
        {
            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { levelsPath });
            levels = new LevelsScriptable[levelsPaths.Length];

            for (int i = 0; i < levelsPaths.Length; i++)
            {

                levels[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelsPaths[i]), typeof(LevelsScriptable)) as LevelsScriptable;

            }

            myTarget.allLevels = levels;
        }
        else
        {
            myTarget.allLevels = new LevelsScriptable[0];
            levels = new LevelsScriptable[0];
        }
    }




    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        DrawGridSizeInspecGUI();

        GUILayout.Space(12);

        DrawLevelDataInspecGUI();

        if (currentLevel != null)
        {
            GUILayout.Space(12);

            DrawEditModesInspecGUI();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }
    }

    private void OnSceneGUI()
    {
        DrawBox();
        DrawLayerGUI();
        DrawLevelGUI();
        DrawStatsGUI();
        DrawModeGUI();
        ModeHandler();
        EventHandler();

        //if (GUI.changed)
        //{
        //    EditorUtility.SetDirty(myTarget);
        //}
    }




    private void DrawLevelDataInspecGUI()
    {
        EditorGUILayout.LabelField("Level", titleStyle);



        EditorGUILayout.BeginVertical("box");

        EditorGUI.BeginChangeCheck();

        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField("Selected Level", myTarget.selectedLevel, typeof(LevelsScriptable), false);

        if (EditorGUI.EndChangeCheck())
        {
            //Undo.RegisterCompleteObjectUndo(myTarget.selectedLevel, "Recording changing scene event");

            if (myTarget.selectedLevel != null)
            {
                InitSelectedLevelValues();
            }
        }



        if (myTarget.selectedLevel != null)
        {
            myTarget.levelCategories = myTarget.selectedLevel.level;


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal("box");

            myTarget.selectedLevel.name = EditorGUILayout.TextField("Level Name", myTarget.selectedLevel.name);

            if (EditorGUI.EndChangeCheck())
            {
                nameSaved = false;
            }

            if (!nameSaved)
            {
                if (GUILayout.Button("Save"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(myTarget.selectedLevel);
                    AssetDatabase.RenameAsset(assetPath, myTarget.selectedLevel.name);
                    nameSaved = true;
                }
            }

            EditorGUILayout.EndHorizontal();

            currentLevel.level.levelSpec.timeForThisLevel = EditorGUILayout.FloatField("Durée du Timer", currentLevel.level.levelSpec.timeForThisLevel);

            if (currentLevel.level.levelSpec.timeForThisLevel == 0)
            {
                EditorGUILayout.HelpBox("Le Timer ne peut être égale à 0", MessageType.Warning);
                EditorGUILayout.HelpBox("Non mais allo quoi ?!", MessageType.Error);
            }

            currentLevel.level.levelSpec.impactRadiusForThisLevel = EditorGUILayout.FloatField("Taille de l'impact", currentLevel.level.levelSpec.impactRadiusForThisLevel);

            if (currentLevel.level.levelSpec.impactRadiusForThisLevel == 0)
            {
                EditorGUILayout.HelpBox("L'impact est égale à 0", MessageType.Warning);
            }

            currentLevel.level.levelSpec.balleSpeedForThisLevel = EditorGUILayout.FloatField("Modifier pour la vitesse de la balle", currentLevel.level.levelSpec.balleSpeedForThisLevel);

            if (currentLevel.level.levelSpec.balleSpeedForThisLevel == 0)
            {
                EditorGUILayout.HelpBox("La vitesse de la balle ne peut être est égale à 0 ... pauvre fou", MessageType.Warning);
            }
        }
        else
            myTarget.levelCategories = null;


        if (myTarget.selectedLevel == null)
        {
            EditorGUILayout.HelpBox("Tu dois attacher un level.asset", MessageType.Warning);
        }


        GUILayout.Space(2);


        EditorGUILayout.EndVertical();
    }

    private void DrawGridSizeInspecGUI()
    {
        EditorGUILayout.LabelField("Grid Parameters", titleStyle);

        EditorGUILayout.BeginVertical("box");

        //mySpace.DoLayoutList();
        serializedObject.Update();

        mySpace.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(1);

        myTarget.xGridPlacement = myTarget.editorSpace[0].x;
        GUILayout.Space(8);
        myTarget.yGridPlacement = myTarget.editorSpace[0].y;
        GUILayout.Space(8);
        myTarget.zGridPlacement = myTarget.editorSpace[0].z;

        EditorGUILayout.BeginVertical("box");

        EditorGUI.BeginDisabledGroup(newCellSize != myTarget.CellSize);
        if (myTarget.editorSpace.Count > 1)
        {
            newTotalColumns = (int)EditorGUILayout.Slider("Number of Rows", newTotalColumns, 0, (int)(myTarget.maxHeightSpace() / myTarget.CellSize));
            newTotalRows = (int)EditorGUILayout.Slider("Number of Columns", newTotalRows, 0, (int)(myTarget.maxWidthSpace() / myTarget.CellSize));
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(newTotalColumns != myTarget.totalRows || newTotalRows != myTarget.totalColumns);
        newCellSize = EditorGUILayout.Slider("Cell Size", newCellSize, 0.1f, 1f);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(2);

        bool OldEnabled = GUI.enabled;
        GUI.enabled = (newTotalColumns != myTarget.totalRows || newTotalRows != myTarget.totalColumns || newCellSize != myTarget.CellSize);

        bool buttonResize = GUILayout.Button("Resize", GUILayout.Height(2 * EditorGUIUtility.singleLineHeight));
        if (buttonResize)
        {
            if (EditorUtility.DisplayDialog(
                "Level Creator",
                "Êtes-vous sûr de vouloir reset le level?\n" +
                "Cette action est IRREVERSIBLE et a pour effet d'EFFACER TOUS LES LEVELS",
                "Ouais", "Nop"))
            {
                ResizeLevels();
            }
        }


        GUILayout.Space(2);


        bool buttonReset = GUILayout.Button("Reset Default Grid");



        if (buttonReset)
        {
            ResetResizeValues();
        }

        GUI.enabled = OldEnabled;


        GUILayout.Space(2);


        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawEditModesInspecGUI()
    {
        EditorGUILayout.LabelField("Brick Parameters", titleStyle);

        GUILayout.Space(2);



        switch (paintMode)
        {
            case PaintMode.OnDraw:
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Color Preset", layerStyle);
                    paintedBrickSettings.brickColorPreset = EditorGUILayout.IntSlider(paintedBrickSettings.brickColorPreset, 0, myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets.Length - 1);

                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal("box");

                    EditorGUILayout.LabelField(myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[paintedBrickSettings.brickColorPreset].tag);

                    EditorGUILayout.ColorField(myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[paintedBrickSettings.brickColorPreset].fresnelColors);
                    EditorGUILayout.ColorField(myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[paintedBrickSettings.brickColorPreset].coreEmissiveColors);

                    GUILayout.EndHorizontal();


                    GUILayout.Space(8);



                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Brick Type", layerStyle);
                    paintedBrickSettings.brickTypePreset = EditorGUILayout.IntSlider(paintedBrickSettings.brickTypePreset, 0, myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets.Length - 1);

                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal("box");

                    EditorGUILayout.LabelField(myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[paintedBrickSettings.brickTypePreset].tag);

                    EditorGUILayout.IntField(myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[paintedBrickSettings.brickTypePreset].armorValue);
                    EditorGUILayout.IntField(myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[paintedBrickSettings.brickTypePreset].scoreValue);

                    GUILayout.EndHorizontal();


                    GUILayout.Space(8);


                    GUILayout.BeginHorizontal("box");

                    if (paintedBrickSettings.isMalus && paintedBrickSettings.isBonus)
                    {
                        EditorGUILayout.LabelField("Malus & Bonus", layerStyle);
                    }
                    else if (paintedBrickSettings.isMalus)
                    {
                        EditorGUILayout.LabelField("Malus", layerStyle);
                    }
                    else if (paintedBrickSettings.isBonus)
                    {
                        EditorGUILayout.LabelField("Bonus", layerStyle);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Bon/Mal-us ?", layerStyle);
                    }

                    paintedBrickSettings.isBonus = EditorGUILayout.Toggle(paintedBrickSettings.isBonus);
                    paintedBrickSettings.isMalus = EditorGUILayout.Toggle(paintedBrickSettings.isMalus);

                    GUILayout.EndHorizontal();


                    GUILayout.Space(8);


                    myTarget.brickWaypoints = new List<Vector3>();


                    if (!paintedBrickSettings.isMoving)
                    {
                        GUILayout.BeginVertical("box");

                        paintedBrickSettings.isMoving = EditorGUILayout.ToggleLeft("Is the Brick Moving ?", paintedBrickSettings.isMoving, layerStyle);

                        GUILayout.EndVertical();
                    }



                    if (paintedBrickSettings.isMoving)
                    {
                        paintedBrickSettings.isMoving = EditorGUILayout.ToggleLeft("Brick Moving parameters", paintedBrickSettings.isMoving, layerStyle);

                        GUILayout.BeginVertical("box");

                        paintedBrickSettings.smoothTime = EditorGUILayout.Slider("smoothTime", paintedBrickSettings.smoothTime, 0.01f, 1f);
                        paintedBrickSettings.speed = EditorGUILayout.Slider("speed", paintedBrickSettings.speed, 0.1f, 10f);

                        GUILayout.EndVertical();
                    }


                    GUILayout.EndVertical();

                    break;
                }

            case PaintMode.OnSelection:
                {
                    if (brickSettingsDisplayed.isBrickHere)
                    {
                        EditorGUI.BeginChangeCheck();

                        GUILayout.BeginVertical("box");
                        GUILayout.BeginHorizontal();

                        EditorGUI.BeginChangeCheck();

                        EditorGUILayout.LabelField("Color Preset", layerStyle);
                        brickSettingsDisplayed.brickColorPreset = EditorGUILayout.IntSlider(brickSettingsDisplayed.brickColorPreset, 0, myTarget.colorPresets[0].colorPresets.Length - 1);

                        if (EditorGUI.EndChangeCheck())
                        {
                            RefreshBrick(brickPosition);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal("box");

                        EditorGUILayout.LabelField(myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[brickSettingsDisplayed.brickColorPreset].tag);

                        EditorGUILayout.ColorField(myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[brickSettingsDisplayed.brickColorPreset].fresnelColors);
                        EditorGUILayout.ColorField(myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[brickSettingsDisplayed.brickColorPreset].coreEmissiveColors);

                        GUILayout.EndHorizontal();


                        GUILayout.Space(8);


                        GUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Brick Type", layerStyle);
                        brickSettingsDisplayed.brickTypePreset = EditorGUILayout.IntSlider(brickSettingsDisplayed.brickTypePreset, 0, myTarget.brickPresets[0].brickPresets.Length - 1);

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal("box");

                        EditorGUILayout.LabelField(myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[brickSettingsDisplayed.brickTypePreset].tag);

                        EditorGUILayout.IntField(myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[brickSettingsDisplayed.brickTypePreset].armorValue);
                        EditorGUILayout.IntField(myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[brickSettingsDisplayed.brickTypePreset].scoreValue);

                        GUILayout.EndHorizontal();


                        GUILayout.Space(8);


                        GUILayout.BeginHorizontal("box");

                        if (brickSettingsDisplayed.isMalus && brickSettingsDisplayed.isBonus)
                        {
                            EditorGUILayout.LabelField("Malus & Bonus", layerStyle);
                        }
                        else if (brickSettingsDisplayed.isMalus)
                        {
                            EditorGUILayout.LabelField("Malus", layerStyle);
                        }
                        else if (brickSettingsDisplayed.isBonus)
                        {
                            EditorGUILayout.LabelField("Bonus", layerStyle);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Bon/Mal-us ?", layerStyle);
                        }

                        brickSettingsDisplayed.isBonus = EditorGUILayout.Toggle(brickSettingsDisplayed.isBonus);
                        brickSettingsDisplayed.isMalus = EditorGUILayout.Toggle(brickSettingsDisplayed.isMalus);

                        GUILayout.EndHorizontal();


                        GUILayout.Space(8);


                        if (!brickSettingsDisplayed.isMoving)
                        {
                            GUILayout.BeginVertical("box");

                            brickSettingsDisplayed.isMoving = EditorGUILayout.ToggleLeft("Is the Brick Moving ?", brickSettingsDisplayed.isMoving, layerStyle);

                            GUILayout.EndVertical();
                        }

                        if (brickSettingsDisplayed.isMoving)
                        {
                            brickSettingsDisplayed.isMoving = EditorGUILayout.ToggleLeft("Is the Brick Moving ?", brickSettingsDisplayed.isMoving, layerStyle);

                            GUILayout.BeginVertical("box");

                            brickSettingsDisplayed.smoothTime = EditorGUILayout.Slider("smoothTime", brickSettingsDisplayed.smoothTime, 0.01f, 1f);
                            brickSettingsDisplayed.speed = EditorGUILayout.Slider("speed", brickSettingsDisplayed.speed, 0.1f, 10f);


                            serializedObject.Update();

                            myTarget.brickWaypoints = brickSettingsDisplayed.waypointsStorage;
                            waypointStorageList.DoLayoutList();

                            serializedObject.ApplyModifiedProperties();

                            GUILayout.EndVertical();
                        }


                        GUILayout.EndVertical();

                        if (EditorGUI.EndChangeCheck())
                        {
                            currentLayer.wallBricks[brickPosition] = brickSettingsDisplayed;
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Tu dois selectionner une BRICK", MessageType.Warning);
                    }

                    break;
                }
        }
    }




    private void ResetLayer()
    {
        for (int i = 0; i < myTarget.bricksOnScreen.Length; i++)
        {
            if (myTarget.bricksOnScreen[i] != null)
            {
                if (currentLayer.wallBricks[i].isMoving)
                {
                    currentLayer.wallBricks[i].waypointsStorage.Clear();
                    DrawWaypointIcon();
                }

                DestroyImmediate(myTarget.bricksOnScreen[i], false);
                BrickSettings blankBrick = new BrickSettings();
                currentLayer.wallBricks[i] = blankBrick;
            }
        }
    }

    private void CleanLayer()
    {
        if (currentLayer != null)
        {
            //Destroy resistant brick of shit (those fuckers that stay under their parent to fuck them up)
            if (myTarget.transform.childCount > 0)
            {
                List<GameObject> go = new List<GameObject>();

                for (int i = 0; i < myTarget.transform.childCount; i++)
                {
                    go.Add(myTarget.transform.GetChild(i).gameObject);
                }

                for (int i = 0; i < go.Count; i++)
                {
                    DestroyImmediate(go[i]);
                }
            }


            myTarget.bricksOnLayer = 0;
            myTarget.numberOfNeutralBrick = 0;
            myTarget.numberOfColoredBrick01 = 0;
            myTarget.numberOfColoredBrick02 = 0;

            myTarget.bricksOnScreen = new GameObject[editorPreset.columns * editorPreset.rows];

            waypointsgo.Clear();

            
        }
    }

    private void SpawnLayer()
    {
        currentLayer = currentLevel.level.levelWallBuilds.walls[selectedLayer];
        myTarget.currentLayer = currentLayer;

        for (int i = 0; i < currentLayer.wallBricks.Count; i++)
        {
            if (currentLayer.wallBricks[i].isBrickHere)
            {
                GameObject obj = Instantiate(prefabBase) as GameObject;
                BrickBehaviours objBehaviours = obj.GetComponent<BrickBehaviours>();
                MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
                Material[] mats = objMesh.sharedMaterials;

                mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
                mats[1].SetFloat("_Metallic", 0.75f);

                mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
                mats[0].SetColor("_FresnelColor", myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[currentLayer.wallBricks[i].brickColorPreset].fresnelColors);
                mats[0].SetColor("_CoreEmissiveColor", myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[currentLayer.wallBricks[i].brickColorPreset].coreEmissiveColors);
                mats[0].SetFloat("_XFrameThickness", 0.75f);
                mats[0].SetFloat("_YFrameThickness", 0.75f);

                objMesh.sharedMaterials = mats;



                obj.transform.parent = myTarget.transform;

                obj.name = currentLayer.wallBricks[i].brickID;

                obj.transform.position = currentLayer.wallBricks[i].brickPosition;


                if (currentLayer.wallBricks[i].isMoving)
                {
                    objBehaviours.isMoving = currentLayer.wallBricks[i].isMoving;
                    objBehaviours.speed = currentLayer.wallBricks[i].speed;
                    objBehaviours.smoothTime = currentLayer.wallBricks[i].smoothTime;
                    objBehaviours.waypoints = new List<Vector3>();

                    for (int j = 0; j < currentLayer.wallBricks[i].waypointsStorage.Count; j++)
                    {
                        objBehaviours.waypoints.Add(currentLayer.wallBricks[i].waypointsStorage[j]);
                    }
                }


                myTarget.bricksOnLayer++;


                switch (currentLayer.wallBricks[i].brickColorPreset)
                {

                    case 0:
                        myTarget.numberOfNeutralBrick++;
                        break;

                    case 1:
                        myTarget.numberOfColoredBrick01++;
                        break;

                    case 2:
                        myTarget.numberOfColoredBrick02++;
                        break;
                }

                myTarget.bricksOnScreen[i] = obj;
            }
        }

        DrawWaypointIcon();

        if (selectedLayer < currentLevel.level.levelWallBuilds.walls.Length - 1)
        {
            for (int i = selectedLayer + 1; i < currentLevel.level.levelWallBuilds.walls.Length; i++)
            {
                //Debug.Log("layer : " + i);
                Wall nextLayer = currentLevel.level.levelWallBuilds.walls[i];

                for (int j = 0; j < nextLayer.wallBricks.Count; j++)
                {
                    if (nextLayer.wallBricks[j].isBrickHere)
                    {
                        GameObject obj = Instantiate(prefabBase) as GameObject;
                        BrickBehaviours objBehaviours = obj.GetComponent<BrickBehaviours>();
                        MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
                        Material[] mats = objMesh.sharedMaterials;

                        mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
                        mats[1].SetFloat("_Metallic", 0.75f);

                        mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
                        mats[0].SetColor("_FresnelColor", myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[nextLayer.wallBricks[i].brickColorPreset].fresnelColors);
                        mats[0].SetColor("_CoreEmissiveColor", myTarget.colorPresets[myTarget.colorPresetSelected].colorPresets[nextLayer.wallBricks[i].brickColorPreset].coreEmissiveColors);
                        mats[0].SetFloat("_XFrameThickness", 0.75f);
                        mats[0].SetFloat("_YFrameThickness", 0.75f);

                        objMesh.sharedMaterials = mats;



                        obj.transform.parent = myTarget.transform;

                        obj.name = nextLayer.wallBricks[j].brickID + " of layer " + i;

                        obj.transform.position = new Vector3(nextLayer.wallBricks[j].brickPosition.x,
                            nextLayer.wallBricks[j].brickPosition.y,
                            nextLayer.wallBricks[j].brickPosition.z + 0.6f);
                    }
                }
            }
        }

        GameObject frontGo = Instantiate(frontWall);
        frontGo.transform.parent = myTarget.transform;

    }

    private void RefreshBrick(int brickPos)
    {
        DestroyImmediate(myTarget.bricksOnScreen[brickPos], false);

        GameObject obj = Instantiate(prefabBase) as GameObject;
        BrickBehaviours objBehaviours = obj.GetComponent<BrickBehaviours>();
        BrickInfo objBrickInfo = obj.GetComponent<BrickInfo>();
        MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
        Material[] mats = objMesh.sharedMaterials;

        mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
        mats[1].SetFloat("_Metallic", 0.75f);

        mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
        mats[0].SetColor("_FresnelColor", myTarget.colorPresets[0].colorPresets[currentLayer.wallBricks[brickPos].brickColorPreset].fresnelColors);
        mats[0].SetColor("_CoreEmissiveColor", myTarget.colorPresets[0].colorPresets[currentLayer.wallBricks[brickPos].brickColorPreset].coreEmissiveColors);
        mats[0].SetFloat("_XFrameThickness", 0.75f);
        mats[0].SetFloat("_YFrameThickness", 0.75f);

        objMesh.sharedMaterials = mats;



        obj.transform.parent = myTarget.transform;

        obj.name = currentLayer.wallBricks[brickPos].brickID;

        obj.transform.position = currentLayer.wallBricks[brickPos].brickPosition;


        objBrickInfo.armorValue = myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[currentLayer.wallBricks[brickPos].brickTypePreset].armorValue;
        objBrickInfo.scoreValue = myTarget.brickPresets[myTarget.brickPresetSelected].brickPresets[currentLayer.wallBricks[brickPos].brickTypePreset].scoreValue;

        if (currentLayer.wallBricks[brickPos].isMoving)
        {
            objBehaviours.isMoving = currentLayer.wallBricks[brickPos].isMoving;
            objBehaviours.speed = currentLayer.wallBricks[brickPos].speed;
            objBehaviours.smoothTime = currentLayer.wallBricks[brickPos].smoothTime;
            objBehaviours.waypoints = new List<Vector3>();

            for (int j = 0; j < currentLayer.wallBricks[brickPos].waypointsStorage.Count; j++)
            {
                objBehaviours.waypoints.Add(currentLayer.wallBricks[brickPos].waypointsStorage[j]);
            }
        }


        myTarget.bricksOnScreen[brickPos] = obj;
    }



    void DrawBox()
    {
        Handles.BeginGUI();
        GUI.Box(new Rect(5, 20, 250, 115), "");
        Handles.EndGUI();
    }

    public void DrawLevelGUI()
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(10, 25, 190, 30));

        EditorGUI.BeginChangeCheck();

        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField(myTarget.selectedLevel, typeof(LevelsScriptable), false);

        if (EditorGUI.EndChangeCheck())
        {
            if (myTarget.selectedLevel != null)
            {
                InitSelectedLevelValues();
            }
            else
            {
                CleanLayer();
            }

            canPaintWaypoint = false;
            brickSettingsDisplayed = new BrickSettings();
        }

        if (myTarget.selectedLevel != null)
            myTarget.levelCategories = myTarget.selectedLevel.level;
        else
            myTarget.levelCategories = null;

        GUILayout.EndArea();


        if (GUI.Button(new Rect(210, 22.5f, 40, 20), new GUIContent("New", "Create a new Level")))
        {
            string path = "Assets/ScriptableObjects/Levels";

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Levels");
            }

            LevelsScriptable newLevel = ScriptableObject.CreateInstance(typeof(LevelsScriptable)) as LevelsScriptable;


            string assetPath = path + "/Level00.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(newLevel, assetPath);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorGUIUtility.PingObject(newLevel);

            newLevel.level.levelWallBuilds = new WallBuilds();
            newLevel.level.levelWallBuilds.walls = new Wall[1];

            currentLevel = newLevel;

            currentLevel.level.levelWallBuilds.walls[0] = new Wall(newTotalColumns * newTotalRows);

            myTarget.selectedLevel = currentLevel;


            numberOfLayers = currentLevel.level.levelWallBuilds.walls.Length;
            totalLayersDisplayed = numberOfLayers - 1;
            selectedLayer = 0;
            currentLayer = currentLevel.level.levelWallBuilds.walls[selectedLayer];



            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { "Assets/ScriptableObjects/Levels" });
            levels = new LevelsScriptable[levelsPaths.Length];

            for (int i = 0; i < levelsPaths.Length; i++)
            {
                levels[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelsPaths[i]), typeof(LevelsScriptable)) as LevelsScriptable;
            }


            myTarget.allLevels = levels;

            canPaintWaypoint = false;
            brickSettingsDisplayed = new BrickSettings();

            CleanLayer();
            SpawnLayer();
        }



        Handles.EndGUI();
    }

    public void DrawLayerGUI()
    {
        Handles.BeginGUI();

        EditorGUI.BeginDisabledGroup(myTarget.selectedLevel == null);

        if (selectedLayer > 0)
        {
            //Passage au layer PRECEDENT
            if (GUI.Button(new Rect(55, 46, 26, 23), new GUIContent("<", "Go to Previous Layer"), btnStyle))
            {
                selectedLayer--;

                currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];

                canPaintWaypoint = false;
                brickSettingsDisplayed = new BrickSettings();

                CleanLayer();
                SpawnLayer();
            }
        }
        else
        {
            //Blocage au PREMIER layer

            if (GUI.Button(new Rect(55, 46, 26, 23), new GUIContent("-", "This is the First Layer"), noneStyle))
            {
                selectedLayer = 0;
            }

        }

        if (selectedLayer < totalLayersDisplayed)
        {
            //Passage au layer SUIVANT
            if (GUI.Button(new Rect(126, 46, 26, 23), new GUIContent(">", "Go to Next Layer"), btnStyle))
            {
                selectedLayer++;

                currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];

                canPaintWaypoint = false;
                brickSettingsDisplayed = new BrickSettings();

                CleanLayer();
                SpawnLayer();
            }
        }
        else
        {
            //Incrémentation du nombre TOTAL de layer
            if (GUI.Button(new Rect(130, 49, 20, 18), new GUIContent("+", "Add a Layer"), btnStyle))
            {
                numberOfLayers++;
                totalLayersDisplayed = numberOfLayers - 1;


                List<Wall> tempWalls = new List<Wall>();

                for (int i = 0; i < numberOfLayers - 1; i++)
                {
                    tempWalls.Add(currentLevel.level.levelWallBuilds.walls[i]);
                }

                currentLevel.level.levelWallBuilds.walls = new Wall[numberOfLayers];

                for (int i = 0; i < numberOfLayers; i++)
                {
                    currentLevel.level.levelWallBuilds.walls[i] = new Wall(newTotalColumns * newTotalRows);
                }


                for (int i = 0; i < numberOfLayers - 1; i++)
                {
                    currentLevel.level.levelWallBuilds.walls[i] = tempWalls[i];
                }

                myTarget.selectedLevel = currentLevel;

                CleanLayer();
                SpawnLayer();
            }
        }

        if (!changingNumberOfLayers)
        {
            if (GUI.Button(new Rect(160, 49, 90, 18), new GUIContent("Reset Layer", "Clean Layer's Data")))
            {
                ResetLayer();
            }
        }


        GUI.Label(new Rect(4, 46, 60, 25), "Layer", layerStyle);


        //Changement du LAYER SELECTIONNE
        EditorGUI.BeginChangeCheck();

        selectedLayer = EditorGUI.IntField(new Rect(74, 49, 22, 18), selectedLayer, layerStyle);

        if (EditorGUI.EndChangeCheck())
        {
            if (selectedLayer > totalLayersDisplayed)
            {
                selectedLayer = totalLayersDisplayed;
            }

            currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];

            CleanLayer();
            SpawnLayer();
        }

        GUI.Label(new Rect(87, 46, 30, 25), "/", slashStyle);



        //Changement du nombre TOTAL de layer
        totalLayersDisplayed = EditorGUI.IntField(new Rect(107, 49, 22, 18), totalLayersDisplayed, layerStyle);

        if (totalLayersDisplayed + 1 != numberOfLayers)
        {
            changingNumberOfLayers = true;

            if (GUI.Button(new Rect(160, 49, 90, 18), new GUIContent("Save changes", "Reset Number of Layers")))
            {
                ResetNumberOfLayers();
            }
        }
        else
        {
            changingNumberOfLayers = false;
        }


        EditorGUI.EndDisabledGroup();


        GUI.backgroundColor = Color.white;
        Handles.EndGUI();
    }

    public void DrawStatsGUI()
    {
        Handles.BeginGUI();

        EditorGUI.BeginDisabledGroup(myTarget.selectedLevel == null);

        GUI.Label(new Rect(23, 70, 30, 30), colorPresets[myTarget.colorPresetSelected].colorPresets[0].tag, layerStyle);
        GUI.Label(new Rect(123, 70, 30, 30), colorPresets[myTarget.colorPresetSelected].colorPresets[1].tag, layerStyle);
        GUI.Label(new Rect(203, 70, 30, 30), colorPresets[myTarget.colorPresetSelected].colorPresets[2].tag, layerStyle);

        GUI.Box(new Rect(66, 75, 30, 20), myTarget.numberOfNeutralBrick.ToString(), slashStyle);
        GUI.Box(new Rect(143, 75, 30, 20), myTarget.numberOfColoredBrick01.ToString(), slashStyle);
        GUI.Box(new Rect(226, 75, 30, 20), myTarget.numberOfColoredBrick02.ToString(), slashStyle);


        EditorGUI.EndDisabledGroup();


        GUI.backgroundColor = Color.white;
        Handles.EndGUI();
    }

    public void DrawModeGUI()
    {
        List<EditionMode> modes = EditorUtilityScene.GetListFromEnum<EditionMode>();
        List<string> modeLabels = new List<string>();

        if (canPaintWaypoint)
        {
            for (int i = 0; i < modes.Count; i++)
            {
                modeLabels.Add(modes[i].ToString());
            }
        }
        else
        {
            for (int i = 0; i < modes.Count - 1; i++)
            {
                modeLabels.Add(modes[i].ToString());
            }
        }


        Handles.BeginGUI();

        EditorGUI.BeginDisabledGroup(myTarget.selectedLevel == null);

        GUILayout.BeginArea(new Rect(10f, 100, 240, 30f));

        selectedMode = (EditionMode)GUILayout.Toolbar((int)currentMode, modeLabels.ToArray(), GUILayout.ExpandHeight(true));

        GUILayout.EndArea();

        EditorGUI.EndDisabledGroup();

        Handles.EndGUI();
    }

    private void ResetNumberOfLayers()
    {
        int oldNumberOfLayers = myTarget.selectedLevel.level.levelWallBuilds.walls.Length;
        numberOfLayers = totalLayersDisplayed + 1;



        List<Wall> tempWalls = new List<Wall>();

        for (int i = 0; i < oldNumberOfLayers; i++)
        {
            tempWalls.Add(currentLevel.level.levelWallBuilds.walls[i]);
        }


        currentLevel.level.levelWallBuilds.walls = new Wall[numberOfLayers];

        for (int i = 0; i < numberOfLayers; i++)
        {
            currentLevel.level.levelWallBuilds.walls[i] = new Wall(newTotalColumns * newTotalRows);
        }

        if (oldNumberOfLayers > numberOfLayers)
        {
            for (int i = 0; i < numberOfLayers; i++)
            {
                currentLevel.level.levelWallBuilds.walls[i] = tempWalls[i];
            }
        }
        else
        {
            for (int i = 0; i < oldNumberOfLayers; i++)
            {
                currentLevel.level.levelWallBuilds.walls[i] = tempWalls[i];
            }
        }



        myTarget.selectedLevel = currentLevel;

        if (selectedLayer > numberOfLayers - 1)
        {
            selectedLayer = numberOfLayers - 1;

            currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];

            canPaintWaypoint = false;
            brickSettingsDisplayed = new BrickSettings();

            CleanLayer();
            SpawnLayer();
        }
        else
        {
            CleanLayer();
            SpawnLayer();
        }
    }



    private void ModeHandler()
    {
        switch (selectedMode)
        {
            case EditionMode.Paint2D:
            case EditionMode.Waypoint:
            case EditionMode.Erase2D:
                Tools.current = Tool.None;
                break;
            case EditionMode.Select:
                Tools.current = Tool.Custom;
                break;
        }

        //Mode Change
        if (selectedMode != currentMode)
        {
            currentMode = selectedMode;
        }
        //Lock in 2D
    }

    private void EventHandler()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Camera camera = SceneView.currentDrawingSceneView.camera;

        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition = new Vector2(mousePosition.x, camera.pixelHeight - mousePosition.y);

        Vector3 worldPos = camera.ScreenToWorldPoint(mousePosition);

        Vector3 gridPos = myTarget.WorldToGridCoordinates(worldPos);

        Event e = Event.current;
        switch (e.keyCode)
        {
            //Debug.Log("Checking");

            case KeyCode.F1:
                {
                    currentMode = (EditionMode)0;
                    RefreshInspector();
                    break;
                }
            case KeyCode.F2:
                {
                    currentMode = (EditionMode)1;
                    RefreshInspector();
                    break;
                }
            case KeyCode.F3:
                {
                    currentMode = (EditionMode)2;
                    RefreshInspector();
                    break;
                }
            case KeyCode.F4:
                {
                    if (canPaintWaypoint)
                    {
                        currentMode = (EditionMode)3;
                        RefreshInspector();
                    }
                    break;
                }
        }


        int col = (int)gridPos.x;
        int row = (int)gridPos.y;

        if (myTarget.IsInsideGridBounds(col, row))
        {

        }


        switch (currentMode)
        {
            case EditionMode.Paint2D:
                SceneView _scene = SceneView.lastActiveSceneView;
                _scene.orthographic = true;


                Quaternion newPos = _scene.rotation;

                newPos.x = 0f;
                newPos.y = 0f;
                newPos.z = 0f;
                newPos.w = 0f;

                _scene.rotation = newPos;

                _scene.Repaint();

                paintMode = PaintMode.OnDraw;

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
                {
                    Paint(col, row);
                }
                break;

            case EditionMode.Erase2D:
                SceneView _scene2 = SceneView.lastActiveSceneView;
                _scene2.orthographic = true;

                Quaternion newPos2 = _scene2.rotation;

                newPos2.x = 0f;
                newPos2.y = 0f;
                newPos2.z = 0f;
                newPos2.w = 0f;

                _scene2.rotation = newPos2;
                _scene2.Repaint();

                paintMode = PaintMode.OnDraw;

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
                {
                    Erase(col, row);
                }
                break;

            case EditionMode.Select:
                SceneView _scene3 = SceneView.lastActiveSceneView;
                _scene3.orthographic = true;

                Quaternion newPos3 = _scene3.rotation;

                newPos3.x = 0f;
                newPos3.y = 0f;
                newPos3.z = 0f;
                newPos3.w = 0f;

                _scene3.rotation = newPos3;
                _scene3.Repaint();

                paintMode = PaintMode.OnSelection;

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Select(col, row);
                }
                break;

            case EditionMode.Waypoint:
                SceneView _scene4 = SceneView.lastActiveSceneView;
                _scene4.orthographic = true;

                Quaternion newPos4 = _scene4.rotation;

                newPos4.x = 0f;
                newPos4.y = 0f;
                newPos4.z = 0f;
                newPos4.w = 0f;

                _scene4.rotation = newPos4;
                _scene4.Repaint();

                paintMode = PaintMode.OnSelection;

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    PaintWaypoint(brickSettingsDisplayed, col, row);
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    EraseWaypoint(brickSettingsDisplayed, col, row);
                }
                break;
        }
    }




    /// <summary>
    /// Paint Waypoint for selected Brick
    /// </summary>
    /// <param name="movingBrick"></param>
    /// <param name="col"></param>
    /// <param name="row"></param>
    private void PaintWaypoint(BrickSettings movingBrick, int col, int row)
    {
        if (!myTarget.IsInsideGridBounds(col, row) || waypointIcon == null || !brickSettingsDisplayed.isMoving)
        {
            return;
        }


        Vector3 newWaypoint = myTarget.GridToWorldPoint(col, row);

        for (int i = 0; i < movingBrick.waypointsStorage.Count; i++)
        {
            if (movingBrick.waypointsStorage[i] == newWaypoint)
            {
                return;
            }
        }


        #region Undo
        Undo.RegisterCompleteObjectUndo(currentLevel, "Recording Selected Name");
        #endregion


        //Paint Waypoint
        movingBrick.waypointsStorage.Add(newWaypoint);

        DrawWaypointIcon();
    }

    /// <summary>
    /// Erase Waypoint for selected Brick
    /// </summary>
    /// <param name="movingBrick"></param>
    /// <param name="col"></param>
    /// <param name="row"></param>
    private void EraseWaypoint(BrickSettings movingBrick, int col, int row)
    {
        if (!myTarget.IsInsideGridBounds(col, row) || waypointIcon == null || !brickSettingsDisplayed.isMoving)
        {
            return;
        }

        Vector3 newWaypoint = myTarget.GridToWorldPoint(col, row);

        for (int i = 0; i < movingBrick.waypointsStorage.Count; i++)
        {
            if (movingBrick.waypointsStorage[i] == newWaypoint)
            {
                #region Undo
                Undo.RegisterCompleteObjectUndo(currentLevel, "Recording Selected Name");
                #endregion

                movingBrick.waypointsStorage.RemoveAt(i);
            }
        }

        DrawWaypointIcon();
    }

    /// <summary>
    /// Draw Waypoint Icons dor everyWaypoint on the layer
    /// </summary>
    private void DrawWaypointIcon()
    {
        for (int i = 0; i < waypointsgo.Count; i++)
        {
            DestroyImmediate(waypointsgo[i]);
        }

        waypointsgo.Clear();

        for (int i = 0; i < currentLayer.wallBricks.Count; i++)
        {
            if (currentLayer.wallBricks[i].isBrickHere)
            {
                if (currentLayer.wallBricks[i].isMoving)
                {
                    for (int j = 0; j < currentLayer.wallBricks[i].waypointsStorage.Count; j++)
                    {
                        Vector3 pos = new Vector3(
                            currentLayer.wallBricks[i].waypointsStorage[j].x,
                        currentLayer.wallBricks[i].waypointsStorage[j].y,
                        currentLayer.wallBricks[i].waypointsStorage[j].z - 0.5f);


                        GameObject obj = Instantiate(waypointIcon) as GameObject;
                        obj.GetComponent<SpriteRenderer>().color = colorPresets[0].colorPresets[currentLayer.wallBricks[i].brickColorPreset].coreEmissiveColors;
                        obj.GetComponentInChildren<TextMeshPro>().text = j.ToString();
                        obj.transform.position = pos;
                        obj.transform.parent = myTarget.transform;


                        waypointsgo.Add(obj);
                    }
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(currentLevel);
    }

    /// <summary>
    /// Paint Brick on layer
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    private void Paint(int col, int row)
    {

        if (!myTarget.IsInsideGridBounds(col, row) || prefabBase == null)
        {
            return;
        }

        //Détermine l'INDEX de la brick
        int selectedBrick = col * myTarget.totalRows + row;

        if (!currentLayer.wallBricks[selectedBrick].isBrickHere)
        {
            //SPAWN et récupération du BEHAVIOUR
            GameObject obj = Instantiate(prefabBase) as GameObject;
            BrickBehaviours objBehaviours = obj.GetComponent<BrickBehaviours>();
            MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
            Material[] mats = objMesh.sharedMaterials;

            mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
            mats[1].SetFloat("_Metallic", 0.75f);

            mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
            mats[0].SetColor("_FresnelColor", myTarget.colorPresets[0].colorPresets[paintedBrickSettings.brickColorPreset].fresnelColors);
            mats[0].SetColor("_CoreEmissiveColor", myTarget.colorPresets[0].colorPresets[paintedBrickSettings.brickColorPreset].coreEmissiveColors);
            mats[0].SetFloat("_XFrameThickness", 0.75f);
            mats[0].SetFloat("_YFrameThickness", 0.75f);

            objMesh.sharedMaterials = mats;

            //ENREGISTREMENT du LEVEL(scriptable) avant l'ajout de la brick
            #region Undo
            Undo.RecordObject(currentLevel, "Recording Selected Name");
            #endregion

            //POSITION de la brick
            obj.transform.parent = myTarget.transform;

            obj.name = string.Format("{0},{1},{2}", col, row, obj.name);

            obj.transform.position = myTarget.GridToWorldPoint(col, row);

            //SET des datas
            BrickSettings brick = new BrickSettings();
            brick = paintedBrickSettings;
            brick.brickID = prefabBase.name;
            brick.brickPosition = myTarget.GridToWorldPoint(col, row);
            brick.isBrickHere = true;

            brick.waypointsStorage = new List<Vector3>();
            brick.waypointsStorage.Add(myTarget.GridToWorldPoint(col, row));

            //ENREGISTREMENT des datas
            currentLayer.wallBricks[selectedBrick] = brick;
            myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer] = currentLayer;

            if (paintedBrickSettings.isMoving)
            {
                canPaintWaypoint = true;
                currentMode = EditionMode.Waypoint;
                brickSettingsDisplayed = currentLayer.wallBricks[selectedBrick];
            }

            //INCREMENTATION des différents datas statistiques
            myTarget.bricksOnLayer++;

            switch (paintedBrickSettings.brickColorPreset)
            {
                case 0:
                    myTarget.numberOfNeutralBrick++;
                    break;

                case 1:
                    myTarget.numberOfColoredBrick01++;
                    break;

                case 2:
                    myTarget.numberOfColoredBrick02++;
                    break;
            }

            //RECUPERATION du gameobject crée
            myTarget.bricksOnScreen[selectedBrick] = obj;

            //ENREGISTRE la création de la BRICK
            Undo.RegisterCreatedObjectUndo(obj, "Registered created Object");
        }

        #region Old

        //obj.hideFlags = HideFlags.HideInHierarchy;

        #endregion
    }

    /// <summary>
    /// Erase brick on layer
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    private void Erase(int col, int row)
    {
        if (!myTarget.IsInsideGridBounds(col, row))
        {
            return;
        }

        int selectedBrick = col * myTarget.totalRows + row;

        if (currentLayer.wallBricks[selectedBrick].isBrickHere)
        {
            GameObject objToDestroy = myTarget.bricksOnScreen[col * myTarget.totalRows + row];

            #region Undo
            //ENREGISTRE la destruction de la BRICK et le LEVEL avant cette dernière
            Undo.DestroyObjectImmediate(objToDestroy);
            Undo.RecordObject(currentLevel, "Recording Selected Name");
            #endregion

            DestroyImmediate(objToDestroy);

            BrickSettings blankBrick = new BrickSettings();

            myTarget.bricksOnLayer--;

            switch (currentLayer.wallBricks[selectedBrick].brickColorPreset)
            {
                case 0:
                    myTarget.numberOfNeutralBrick--;
                    break;

                case 1:
                    myTarget.numberOfColoredBrick01--;
                    break;

                case 2:
                    myTarget.numberOfColoredBrick02--;
                    break;
            }

            if (currentLayer.wallBricks[selectedBrick].isMoving)
            {
                currentLayer.wallBricks[col * myTarget.totalRows + row].waypointsStorage.Clear();
                DrawWaypointIcon();
            }

            currentLayer.wallBricks[col * myTarget.totalRows + row] = blankBrick;
        }
    }

    private void Select(int col, int row)
    {
        if (!myTarget.IsInsideGridBounds(col, row))
        {
            return;
        }

        int selectedBrick = col * myTarget.totalRows + row;

        if (!currentLayer.wallBricks[selectedBrick].isBrickHere)
        {
            return;
        }

        #region Undo
        //Undo.RecordObject(this, "Recording Selected Level Choice");
        //Undo.RecordObject(myTarget, "Recording Selected Level Choice");
        //Undo.RecordObject(myTarget.selectedLevel, "Recording Selected Name");
        #endregion

        brickSettingsDisplayed = currentLayer.wallBricks[selectedBrick];
        brickPosition = selectedBrick;

        if (brickSettingsDisplayed.isMoving)
        {
            canPaintWaypoint = true;
        }
        else
        {
            canPaintWaypoint = false;
        }

        RefreshInspector();
    }




    private void ResetResizeValues()
    {
        newTotalColumns = myTarget.totalRows;
        newTotalRows = myTarget.totalColumns;
        newCellSize = myTarget.CellSize;

        EditorUtility.SetDirty(myTarget);
    }

    private void ResizeLevels()
    {
        CleanLayer();

        ///Efface tous les levels,
        if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Levels"))
        {
            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { levelsPath });

            for (int j = 0; j < levelsPaths.Length; j++)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(levelsPaths[j]));
            }


            myTarget.allLevels = levels;

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }


        myTarget.allLevels = levels;

        editorPreset.cellSize = newCellSize;
        editorPreset.rows = newTotalColumns;
        editorPreset.columns = newTotalRows;

        editorPreset.editorSpaceRecorded = myTarget.editorSpace;


        myTarget.CellSize = editorPreset.cellSize;
        myTarget.totalColumns = editorPreset.columns;
        myTarget.totalRows = editorPreset.rows;

        InitGridValues();



        float newScale = ((newCellSize * 1.524f) / 0.3f);

        prefabBase.transform.localScale = new Vector3(newScale, newScale, newScale);
        PrefabUtility.ApplyObjectOverride(prefabBase, prefabPath, InteractionMode.UserAction);

        EditorUtility.SetDirty(editorPreset);
    }




    void MyUndoCallBack()
    {
        RefreshInspector();

        Soil();

        DrawWaypointIcon();
    }

    void RefreshInspector()
    {
        LevelInspectorScript[] cew = Resources.FindObjectsOfTypeAll(typeof(LevelInspectorScript)) as LevelInspectorScript[];
        if (cew.Length != 0)
        {

            for (int i = 0; i < cew.Length; i++)
            {
                cew[i].Repaint();
            }
        }
    }

    void Soil()
    {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //EditorUtility.SetDirty(currentLevel);

        for (int i = 0; i < levels.Length; i++)
        {
            EditorUtility.SetDirty(levels[i]);
        }
    }
}
