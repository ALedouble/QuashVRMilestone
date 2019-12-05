using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public LevelsScriptable[] registeredLevels;
    LevelsScriptable currentLevel;
    LevelSettings currentLevelConfig;
    public string levelsPath = "Assets/ScriptableObjects/Levels";


    [Header("Level Parameters")]
    public int debugThisLevel;
    public int numberOfPlayers;

    public float layerDiffPosition = 0.6f;
    public List<Transform> levelTrans;
    public Vector3 posDiffPerPlayer;

    [HideInInspector] public int[] currentLayer;
    bool[] isThereAnotherLayer;
    [HideInInspector] public Vector3[] startPos;
    Vector3[] NextPos;
    Vector3 refVector;
    bool[] changePositionReady;

    [Range(0, 1)] public float smoothTime;
    [Range(5f, 10)] public float sMaxSpeed;

    


    public static LevelManager Instance;



    private void Awake()
    {
        Instance = this;

        ConfigDistribution(debugThisLevel);
        initValues();
    }

    private void Start()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            SetNextLayer(i);
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

    void initValues()
    {
        currentLayer = new int[numberOfPlayers];
        isThereAnotherLayer = new bool[numberOfPlayers];
        startPos = new Vector3[numberOfPlayers];
        NextPos = new Vector3[numberOfPlayers];
        changePositionReady = new bool[numberOfPlayers];
        BrickManager.Instance.currentBricksOnLayer = new int[numberOfPlayers];

        for (int i = 0; i < numberOfPlayers; i++)
        {
            currentLayer[i] = -1;
            isThereAnotherLayer[i] = true;
            startPos[i] = LevelManager.Instance.posDiffPerPlayer * i;
            changePositionReady[i] = false;
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



    /// <summary>
    /// Set up parameters to change level position
    /// </summary>
    public void SetNextLayer(int playerID)
    {
        if (isThereAnotherLayer[playerID])
        {
            currentLayer[playerID] += 1;
            NextPos[playerID] = new Vector3(startPos[playerID].x, startPos[playerID].y, startPos[playerID].z - (layerDiffPosition * currentLayer[playerID]));


            changePositionReady[playerID] = true;

            BrickManager.Instance.SpawnLayer(playerID);
        }


        if (currentLayer[playerID] >= currentLevelConfig.levelWallBuilds.walls.Length - 1)
        {
            isThereAnotherLayer[playerID] = false;
        }
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
}
