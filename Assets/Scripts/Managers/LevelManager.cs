using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public LevelsScriptable[] registeredLevels;
    public LevelsScriptable currentLevel;
    public LevelSettings currentLevelConfig;
    public string levelsPath = "Assets/ScriptableObjects/Levels";

    [Header("Level Parameters")]
    public int currentLayer = -1;
    [SerializeField] bool isThereAnotherLayer = true;

    public float layerDiffPosition;

    public Transform levelTrans;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 NextPos;

    public Vector3 refVector;
    [Range(0, 1)] public float smoothTime;
    [Range(5f, 10)] public float sMaxSpeed;

    public bool changePositionReady = false;


    public static LevelManager Instance;



    private void Awake()
    {
        Instance = this;

        ConfigDistribution(0);
    }

    private void Update()
    {
        if (changePositionReady)
        {
            NextLayer();
        }
    }



    /// <summary>
    /// Go to the next Layer
    /// </summary>
    void NextLayer()
    {
        levelTrans.position = Vector3.SmoothDamp(levelTrans.position, NextPos, ref refVector, smoothTime, sMaxSpeed);

        if (levelTrans.position == NextPos)
        {
            changePositionReady = false;
        }
    }



    /// <summary>
    /// Set up parameters to change level position
    /// </summary>
    public void SetNextLayer()
    {
        if (isThereAnotherLayer)
        {
            currentLayer += 1;
            NextPos = new Vector3(0, 0, startPos.z - (layerDiffPosition * currentLayer));


            changePositionReady = true;

            BrickManager.Instance.SpawnLayer();
        }



        if (currentLayer >= currentLevelConfig.levelWallBuilds.walls.Length)
        {
            isThereAnotherLayer = false;
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

        BrickManager.Instance.levelWallsConfig = currentLevelConfig.levelWallBuilds;
    }
}
