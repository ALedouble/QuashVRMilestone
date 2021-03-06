﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public delegate void OnLayerEnd();

    public event OnLayerEnd onLayerEndEvent;


    public string CurrentLevelID
    {
        get
        {
            if (currentLevel == null)
                return "666";

            if (currentLevel.level.levelSpec.buttonName != null && currentLevel.level.levelSpec.buttonName != "" && currentLevel.level.levelSpec.buttonName != " ")
                return currentLevel.level.levelSpec.buttonName;
            else
            {
                if (currentLevel.level.levelProgression.levelNumber < 10)
                {
                    return "0" + currentLevel.level.levelProgression.levelNumber.ToString();
                }
                else
                {
                    return currentLevel.level.levelProgression.levelNumber.ToString();
                }
            }
        }
    }

    [Header("Récupération de la configuration du level")]
    public LevelsScriptable[] registeredLevels;
    public LevelsScriptable currentLevel;
    [HideInInspector] public LevelSettings currentLevelConfig;
    [HideInInspector] public string levelsPath = "Assets/ScriptableObjects/Levels";
    protected int AndTheWinnerIs;
    public AudioSource music;

    [Header("Level Parameters")]
    public int debugThisLevel;
    public int numberOfPlayers;

    [Header("Shaking")]
    public Shake layerShake;

    [Header("Room Parameters")]
    public float layerDiffPosition = 0.6f;
    public int numberOfLayerToDisplay = 1;
    [HideInInspector] public Transform[] levelTrans;
    [HideInInspector] public Parenting[] playersParents;
    [HideInInspector] public Shakers[] playersShakers;
    [HideInInspector] public LayerCompletedEffect[] playersLayerCompletedFX;
    [HideInInspector] public WinManagerVFX[] playersWinFX;
    [HideInInspector] public MeshRenderer[] allMeshes;
    [HideInInspector] public MeshRenderer midMesh;
    [HideInInspector] public MeshCollider midCollider;

    /*[HideInInspector]*/
    [HideInInspector] public UIlayers[] playersUIlayers;
    [HideInInspector] public Shaker roomShaker;
    [HideInInspector] public GUIHUD playersHUD;
    [HideInInspector] public GUIMenuPause guiMenuPause;
    [HideInInspector] public PlayroomElements playroomElements;

    public Vector3 startPos4Player1;
    public Vector3 posDiffPerPlayer;
    public EditorScriptable editorPreset;

    [HideInInspector] public int[] currentLayer;
    [HideInInspector] public int numberOfLayers;
    UI_LayerBehaviour[] UiLayers;
    bool[] isThereAnotherLayer;
    [HideInInspector] public Vector3[] startPos;
    Vector3[] NextPos;
    Vector3 refVector;
    bool[] changePositionReady;
    [HideInInspector] public bool[] isEverythingDisplayed;
    bool[] firstSetUpDone;

    [Header("Layers Parameters")]
    [Range(0.01f, 1f)] public float smoothTime;
    [Range(2f, 10f)] public float sMaxSpeed;
    public int layersPerRow = 16;
    public float layersRowsOffset = -0.15f;

    public PresetScriptable[] colorPresets { get => BrickManager.Instance.colorPresets; set => BrickManager.Instance.colorPresets = value; }            // Desole...

    public static LevelManager instance;

    private void Awake()
    {
        instance = this;

        //StartLevelInitialization(debugThisLevel);
    }


    public void StartLevelInitialization(int levelToInit)
    {
        ConfigDistribution(levelToInit);
        InitValues();
    }

    public void StartLevelInitialization(LevelsScriptable levelToInit)
    {
        ConfigDistribution(levelToInit);
        music.clip = levelToInit.level.levelSpec.musicForThisLevel;
        music.Play();
        InitValues();
    }

    private void Start()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            SetNextLayer(i);

            for (int j = 0; j < numberOfLayerToDisplay; j++)
            {
                if (!isEverythingDisplayed[i])
                {
                    BrickManager.Instance.SpawnLayer(i, j);
                }
            }

            for (int k = currentLayer[i]; k < numberOfLayerToDisplay; k++)
            {
                if (k <= currentLevel.level.levelWallBuilds.walls.Length - 1)
                {
                    SetWaypoints(i, k);
                }
            }

            BrickManager.Instance.SetCurrentBrickOnLayer(i);
            BrickManager.Instance.SetCurrentActiveLayerBricks(i);
        }
    }

    private void Update()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (changePositionReady[i])
            {
                NextLayer(i);
            }
        }
    }

    /// <summary>
    /// Set HUD depending on the number of Players
    /// </summary>
    void InitRoom()
    {
        playersHUD = new GUIHUD();                                                                                                                              //Problem Here! (Sert à rien)
        roomShaker = new Shaker();                                                                                                                              //Problem Here! (Sert à rien)

        Vector3 roomPos = new Vector3(
                startPos4Player1.x,
                startPos4Player1.y,
                startPos4Player1.z);

        GameObject goHUD = PoolManager.instance.SpawnFromPool("HUD_0" + (numberOfPlayers - 1), roomPos, Quaternion.identity);
        //Debug.Log("Pool HUD " + (numberOfPlayers - 1) + " Player");
        GameObject goRoom = PoolManager.instance.SpawnFromPool("Playroom_0" + (numberOfPlayers - 1), roomPos, Quaternion.identity);

        if (currentLevel.level.levelSpec.goToSpawn != null)
            Instantiate(currentLevel.level.levelSpec.goToSpawn);

        playersHUD = goHUD.GetComponent<GUIHUD>();                                                                                                              //Problem Here! (Ok en fait)
        guiMenuPause = goHUD.GetComponentInChildren<GUIMenuPause>(true);
        roomShaker = goRoom.GetComponent<Shaker>();                                                                                                             //Problem Here! (Ok en fait)
        playroomElements = goRoom.GetComponent<PlayroomElements>();

        allMeshes = playroomElements.renderers;

        if (numberOfPlayers > 1)
        {
            midMesh = playroomElements.midWallRenderer;
            midCollider = playroomElements.midCollider;
        }

        //TimeManager.Instance.SetupTimerGUI(playersHUD.TimerData[0]);
    }

    /// <summary>
    /// Set values depending on the number of Players
    /// </summary>
    void InitValues()
    {
        levelTrans = new Transform[numberOfPlayers];
        currentLayer = new int[numberOfPlayers];
        isThereAnotherLayer = new bool[numberOfPlayers];
        startPos = new Vector3[numberOfPlayers];
        NextPos = new Vector3[numberOfPlayers];
        playersLayerCompletedFX = new LayerCompletedEffect[numberOfPlayers];
        playersWinFX = new WinManagerVFX[numberOfPlayers];

        changePositionReady = new bool[numberOfPlayers];
        isEverythingDisplayed = new bool[numberOfPlayers];
        BrickManager.Instance.currentLayersBrickCount = new int[numberOfPlayers];
        playersParents = new Parenting[numberOfPlayers];
        playersShakers = new Shakers[numberOfPlayers];
        playersUIlayers = new UIlayers[numberOfPlayers];
        firstSetUpDone = new bool[numberOfPlayers];

        ScoreManager.Instance.displayedScore = new GUIScoreData[numberOfPlayers];

        ScoreManager.Instance.displayedCombo = new GUIComboData[numberOfPlayers];
        ScoreManager.Instance.score = new float[numberOfPlayers];
        ScoreManager.Instance.combo = new int[numberOfPlayers];
        ScoreManager.Instance.brickCounterGauge = new int[numberOfPlayers];
        ScoreManager.Instance.playersMaxCombo = new int[numberOfPlayers];
        ExplosionManager.Instance.PlayersExplosionRadius = new float[numberOfPlayers];


        TimeManager.Instance.SetNewTimer(currentLevel.level.levelSpec.timePerLayer);

        //TimeManager.Instance.LevelMaxTime = currentLevel.level.levelSpec.timeForThisLevel;
        //TimeManager.Instance.CurrentTimer = 0;

        InitRoom();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            currentLayer[i] = -1;
            isThereAnotherLayer[i] = true;
            startPos[i] = posDiffPerPlayer * i;

            //Debug.Log("Walls LENGTH : " + currentLevel.level.levelWallBuilds.walls.Length);
            playersParents[i].layersParent = new Transform[currentLevel.level.levelWallBuilds.walls.Length];

            numberOfLayers = currentLevel.level.levelWallBuilds.walls.Length;
            //Debug.Log("numberOfLayers : " + numberOfLayers);

            playersUIlayers[i].layersUI = new UI_LayerBehaviour[numberOfLayers];
            playersLayerCompletedFX[i] = playroomElements.playersLayersCompletedEffect[i];
            playersWinFX[i] = playroomElements.playersWinEffect[i];

            Vector3 goPos = new Vector3(startPos4Player1.x + (posDiffPerPlayer.x * i), startPos4Player1.y + (posDiffPerPlayer.y * i), startPos4Player1.z + (posDiffPerPlayer.z * i));
            GameObject trans = PoolManager.instance.SpawnFromPool("LevelParents", new Vector3(0, 0, 0), Quaternion.identity);
            trans.transform.position = goPos;
            trans.name = "Wall_Of_Player_" + i;
            levelTrans[i] = trans.transform;

            //SOLO ONLY
            if (i == 0 && GameManager.Instance.offlineMode)
            {
                ScoreManager.Instance.displayedScore[i] = playersHUD.ScoreDATAs[i];

                //playersHUD.ScoreConditionParents[0].SetActive(true);


                //Set text value for levelMax timer
                string textForLevelMax = "";

                int minutesForLevelMax = (int)TimeManager.Instance.LevelMaxTime / 60;
                int secondsForLevelMax = (int)TimeManager.Instance.LevelMaxTime - (minutesForLevelMax * 60);

                if (minutesForLevelMax < 10)
                {
                    if (secondsForLevelMax < 10)
                    {
                        textForLevelMax = "0" + minutesForLevelMax + ":" + "0" + secondsForLevelMax;
                        playersHUD.TimerData[1].UpdateText(textForLevelMax);
                    }
                    else
                    {
                        textForLevelMax = "0" + minutesForLevelMax + ":" + secondsForLevelMax;
                        playersHUD.TimerData[1].UpdateText(textForLevelMax);
                    }
                }
                else
                {
                    if (secondsForLevelMax < 10)
                    {
                        textForLevelMax = minutesForLevelMax + ":" + "0" + secondsForLevelMax;
                        playersHUD.TimerData[1].UpdateText(textForLevelMax);
                    }
                    else
                    {
                        textForLevelMax = minutesForLevelMax + ":" + secondsForLevelMax;
                        playersHUD.TimerData[1].UpdateText(textForLevelMax);
                    }
                }



                int length = currentLevel.level.levelProgression.numberOfAdditionalConditions;

                if (length > 0)
                {
                    bool isScoreType = false;
                    bool isComboType = false;
                    bool isTimerType = false;

                    for (int y = 0; y < length; y++)
                    {
                        switch (currentLevel.level.levelProgression.conditionsToComplete[y].conditionType)
                        {
                            case CompleteConditionType.Score:
                                if (!isScoreType)
                                {
                                    ScoreManager.Instance.displayedScore[i] = playersHUD.ScoreDATAs[1];
                                    playersHUD.ScoreConditionParents[1].SetActive(true);

                                    string conditionScore = currentLevel.level.levelProgression.conditionsToComplete[y].conditionReachedAt.ToString();
                                    playersHUD.ScoreConditionData.UpdateText(conditionScore);
                                    isScoreType = true;

                                    ScoreManager.Instance.isThereScoreCondition = true;
                                    ScoreManager.Instance.scoreConditionValue = currentLevel.level.levelProgression.conditionsToComplete[y].conditionReachedAt;
                                }
                                break;

                            case CompleteConditionType.Combo:
                                if (!isComboType)
                                {
                                    ScoreManager.Instance.isThereComboCondition = true;
                                    ScoreManager.Instance.comboConditionValue = currentLevel.level.levelProgression.conditionsToComplete[y].conditionReachedAt;
                                }
                                break;

                            case CompleteConditionType.Timing:
                                if (!isTimerType)
                                {
                                    playersHUD.TimerConditionParents[1].SetActive(true);

                                    TimeManager.Instance.timerConditionValue = currentLevel.level.levelProgression.conditionsToComplete[y].conditionReachedAt;
                                    TimeManager.Instance.isThereTimerCondition = true;

                                    string text = "";

                                    int minutes = (int)TimeManager.Instance.timerConditionValue / 60;
                                    int seconds = (int)TimeManager.Instance.timerConditionValue - (minutes * 60);

                                    if (minutes < 10)
                                    {
                                        if (seconds < 10)
                                        {
                                            text = "0" + minutes + ":" + "0" + seconds;
                                            playersHUD.TimerConditionData.UpdateText(text);
                                        }
                                        else
                                        {
                                            text = "0" + minutes + ":" + seconds;
                                            playersHUD.TimerConditionData.UpdateText(text);
                                        }
                                    }
                                    else
                                    {
                                        if (seconds < 10)
                                        {
                                            text = minutes + ":" + "0" + seconds;
                                            playersHUD.TimerConditionData.UpdateText(text);
                                        }
                                        else
                                        {
                                            text = minutes + ":" + seconds;
                                            playersHUD.TimerConditionData.UpdateText(text);
                                        }
                                    }

                                    isTimerType = true;
                                }
                                break;
                        }

                    }

                    if (!isScoreType)
                    {
                        playersHUD.ScoreConditionParents[0].SetActive(true);
                        ScoreManager.Instance.displayedScore[i] = playersHUD.ScoreDATAs[i];
                    }

                }
                else
                {
                    playersHUD.ScoreConditionParents[0].SetActive(true);

                    ScoreManager.Instance.displayedScore[i] = playersHUD.ScoreDATAs[i];
                }

                TimeManager.Instance.SetupTimerGUI(playersHUD.TimerData[0]);
            }
            else
            {
                ScoreManager.Instance.displayedScore[i] = playersHUD.ScoreDATAs[i];
                TimeManager.Instance.SetupTimerGUI(playersHUD.TimerData[0]);
            }

            ScoreManager.Instance.displayedCombo[i] = playersHUD.ComboData[i];
            ScoreManager.Instance.combo[i] = 1;
            ScoreManager.Instance.playersMaxCombo[i] = 1;


            ///Layers UI logic
            int layersOnThisRow = 0;
            int currentLayersRow = 0;
            float rowOffset = 0f;

            if (numberOfLayers < layersPerRow)
            {
                playersHUD.layerCountParent[i].localPosition = new Vector3(0 - (0.1f * numberOfLayers), 0.5f, 0);

                if (numberOfLayers % 2 == 0)
                    rowOffset = 0.3f;

            }
            else
            {
                playersHUD.layerCountParent[i].localPosition = new Vector3(0 - (0.1f * layersPerRow), 0.5f, 0);
            }


            for (int r = 0; r < numberOfLayers; r++)
            {
                if (layersOnThisRow >= layersPerRow)
                {
                    currentLayersRow++;
                    layersOnThisRow = 0;
                    int numberOfLayersForNextRow = numberOfLayers - (layersPerRow * currentLayersRow);


                    if (numberOfLayersForNextRow > layersPerRow)
                        numberOfLayersForNextRow = layersPerRow;


                    if ((numberOfLayersForNextRow % 2) == 1)
                        rowOffset = 0;
                    else
                        rowOffset = 0.3f;
                }

                GameObject layerUI = PoolManager.instance.SpawnFromPool("LayerUI", new Vector3(0, 0), Quaternion.identity);
                layerUI.transform.parent = playersHUD.layerCountParent[i];
                layerUI.transform.localPosition = new Vector3(0 + (0.2f * layersOnThisRow) + rowOffset, 0 - (layersRowsOffset * currentLayersRow), 0);
                playersUIlayers[i].layersUI[r] = layerUI.GetComponent<UI_LayerBehaviour>();

                layersOnThisRow++;
            }



            ExplosionManager.Instance.PlayersExplosionRadius[i] = currentLevel.level.levelSpec.impactRadiusForThisLevel;

            playersShakers[i].layersShaker = new Shaker[playersParents[i].layersParent.Length];

            //Spawn les PARENTS pour chaque LAYER du mur d'un joueur
            for (int j = 0; j < playersParents[i].layersParent.Length; j++)
            {
                GameObject obj = PoolManager.instance.SpawnFromPool("LayerParents", new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.parent = levelTrans[i];
                obj.transform.localPosition = Vector3.zero;
                obj.name = i + "_" + j;
                Shaker s = obj.AddComponent<Shaker>();
                playersShakers[i].layersShaker[j] = obj.GetComponent<Shaker>(); //Add shaker
                playersParents[i].layersParent[j] = obj.transform;
            }
        }
    }


    /// <summary>
    /// Go to the next Layer
    /// </summary>
    void NextLayer(int playerID)
    {
        levelTrans[playerID].position = Vector3.SmoothDamp(levelTrans[playerID].position, NextPos[playerID], ref refVector, smoothTime, sMaxSpeed);

        if (levelTrans[playerID].position == NextPos[playerID])
        {
            changePositionReady[playerID] = false;
        }
    }


    void EndOfLayerUpdates(int playerID, int layerCompleted)
    {
        //Debug.Log("The completed layer is : " + layerCompleted);
        playersUIlayers[playerID].layersUI[layerCompleted].CompleteLayer();

        // Rajout d'animation FIN d'un layer
        playersLayerCompletedFX[playerID].StartEffect();


        // Play SFX Level Completed
        AudioManager.instance.PlaySound("Layer_Completed", Vector3.zero);

        if (!isThereAnotherLayer[playerID])
        {
            AndTheWinnerIs = playerID;
            playersWinFX[playerID].PlayVFX();
            GameManager.Instance.EndOfTheGame(playerID);
        }
    }

    /// <summary>
    /// Deactivate all layers
    /// </summary>
    public void CleanWalls()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            for (int j = 0; j < levelTrans[i].childCount; j++)
            {
                for (int k = 0; k < levelTrans[i].GetChild(j).childCount; k++)
                    levelTrans[i].GetChild(j).GetChild(k).gameObject.SetActive(false);

                levelTrans[i].GetChild(j).gameObject.SetActive(false);
            }

            levelTrans[i].gameObject.SetActive(false);
        }
    }

    public void OnTimerNextLayer()
    {
        SetNextLayer(0);
    }

    /// <summary>
    /// Set up parameters to change/updtae wall position
    /// </summary>
    public void SetNextLayer(int playerID)
    {
        if (onLayerEndEvent != null && isThereAnotherLayer[playerID])
            onLayerEndEvent();


        if (isThereAnotherLayer[playerID])
        {
            currentLayer[playerID]++;
            NextPos[playerID] = new Vector3(startPos[playerID].x, startPos[playerID].y, startPos[playerID].z - (layerDiffPosition * currentLayer[playerID]));

            //Check si le joueur est le premier à avoir fini cette couche. La récompense est divisé selon son "placement"
            if (firstSetUpDone[playerID])
            {
                float rewardModifier = 1;

                for (int i = 0; i < numberOfPlayers; i++)
                {
                    if (playerID != i && currentLayer[playerID] < currentLayer[i])
                    {
                        rewardModifier++;
                    }
                }
            }


            if (!isEverythingDisplayed[playerID] && firstSetUpDone[playerID])
            {
                BrickManager.Instance.SpawnLayer(playerID, numberOfLayerToDisplay - 1);
            }

            //Move the wall forward to the next layer
            StartCoroutine(MoveWallForward(playerID));
        }

        if (firstSetUpDone[playerID])
        {
            EndOfLayerUpdates(playerID, currentLayer[playerID] - 1);

            if (isThereAnotherLayer[playerID] && currentLevel.level.levelSpec.timeAttack)
                TimeManager.Instance.OnTimeAttackBoost();
        }

        if (currentLayer[playerID] >= currentLevelConfig.levelWallBuilds.walls.Length - 1)
        {
            isThereAnotherLayer[playerID] = false;
        }
        //Debug.Log("NumberOfPlayers : " + playersParents.Length);
        //Debug.Log("NumberOf Layers : " + playersParents[playerID].layersParent.Length);

        firstSetUpDone[playerID] = true;
    }


    /// <summary>
    /// Set les waypoints selon la nouvelle position du layer
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="layer"></param>
    public void SetWaypoints(int playerID, int layer)
    {
        if (playersParents[playerID].layersParent[layer].childCount > 0)
        {
            List<GameObject> go = new List<GameObject>();

            for (int i = 0; i < playersParents[playerID].layersParent[layer].childCount; i++)
            {
                GameObject obj = playersParents[playerID].layersParent[layer].GetChild(i).gameObject;
                if (obj.GetComponent<BrickBehaviours>().isAMovingBrick)
                {
                    go.Add(obj);
                }
            }

            for (int i = 0; i < go.Count; i++)
            {
                BrickBehaviours brick = go[i].GetComponent<BrickBehaviours>();


                for (int j = 0; j < BrickManager.Instance.levelWallsConfig.walls[layer].wallBricks[brick.savedInIndex].waypointsStorage.Count; j++)
                {
                    Vector3 waypointToLayer = new Vector3(
                        BrickManager.Instance.levelWallsConfig.walls[layer].wallBricks[brick.savedInIndex].waypointsStorage[j].x + (posDiffPerPlayer.x * playerID),
                        BrickManager.Instance.levelWallsConfig.walls[layer].wallBricks[brick.savedInIndex].waypointsStorage[j].y + (posDiffPerPlayer.y * playerID),
                        BrickManager.Instance.levelWallsConfig.walls[layer].wallBricks[brick.savedInIndex].waypointsStorage[j].z + (posDiffPerPlayer.z * playerID) + layerDiffPosition * (float)(layer - currentLayer[playerID]));

                    brick.waypoints[j] = waypointToLayer;
                }
            }
        }
    }

    public void ShakeLayer(int wallID)
    {
        LevelManager.instance.playersShakers[wallID].layersShaker[currentLayer[wallID]].PlayShake(layerShake);
        roomShaker?.PlayShake();
    }

    /// <summary>
    /// Attribute data to corresponding managers
    /// </summary>
    /// <param name="selectedLevel"></param>
    public void ConfigDistribution(int selectedLevel)
    {
        currentLevel = registeredLevels[selectedLevel];
        currentLevelConfig = currentLevel.level;
        BrickManager.Instance.levelWallsConfig = registeredLevels[selectedLevel].level.levelWallBuilds;
    }

    /// <summary>
    /// Attribute data to corresponding managers
    /// </summary>
    /// <param name="selectedLevel"></param>
    public void ConfigDistribution(LevelsScriptable selectedLevel)
    {
        currentLevel = selectedLevel;
        currentLevelConfig = currentLevel.level;
        BrickManager.Instance.levelWallsConfig = selectedLevel.level.levelWallBuilds;
    }

    IEnumerator MoveWallForward(int playerID)
    {
        BrickManager.Instance.SetCurrentBrickOnLayer(playerID);

        yield return new WaitForSeconds(0.90f);

        BrickManager.Instance.ActivateMovingBricks(playerID);

        changePositionReady[playerID] = true;

        for (int i = currentLayer[playerID]; i < (currentLayer[playerID] + numberOfLayerToDisplay - 1); i++)
        {
            if (i <= currentLevel.level.levelWallBuilds.walls.Length - 1)
            {
                SetWaypoints(playerID, i);
            }
        }
    }




    [System.Serializable]
    public struct Parenting
    {
        public Transform[] layersParent;
    }

    [System.Serializable]
    public struct Shakers
    {
        public Shaker[] layersShaker;
    }

    [System.Serializable]
    public struct UIlayers
    {
        public UI_LayerBehaviour[] layersUI;
    }
}
