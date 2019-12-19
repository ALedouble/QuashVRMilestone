using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public LevelsScriptable[] registeredLevels;
    public LevelsScriptable currentLevel;
    public LevelSettings currentLevelConfig;
    [HideInInspector] public string levelsPath = "Assets/ScriptableObjects/Levels";


    [Header("Level Parameters")]
    public int debugThisLevel;
    public int numberOfPlayers;

    public float layerDiffPosition = 0.6f;
    public int numberOfLayerToDisplay = 1;
    [HideInInspector] public Transform[] levelTrans;
    [HideInInspector] public Parenting[] playersParents;
    [HideInInspector] public Shakers[] playersShakers;
    [HideInInspector] public Shaker roomShaker;
    [HideInInspector] public GUIHUD playersHUD;

    public Vector3 startPos4Player1;
    public Vector3 posDiffPerPlayer;
    public EditorScriptable editorPreset;

    [HideInInspector] public int[] currentLayer;
    bool[] isThereAnotherLayer;
    [HideInInspector] public Vector3[] startPos;
    Vector3[] NextPos;
    Vector3 refVector;
    bool[] changePositionReady;
    [HideInInspector] public bool[] isEverythingDisplayed;
    bool[] firstSetUpDone;

    [Range(0.01f, 1f)] public float smoothTime;
    [Range(2f, 10f)] public float sMaxSpeed;


    public static LevelManager instance;



    private void Awake()
    {
        instance = this;

        ConfigDistribution(debugThisLevel);
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
        playersHUD = new GUIHUD();
        roomShaker = new Shaker();

        Vector3 roomPos = new Vector3(
                startPos4Player1.x,
                startPos4Player1.y,
                startPos4Player1.z);

        GameObject goHUD = PoolManager.instance.SpawnFromPool("HUD_0" + (numberOfPlayers - 1), roomPos, Quaternion.identity);
        GameObject goRoom = PoolManager.instance.SpawnFromPool("Playroom_0" + (numberOfPlayers - 1), roomPos, Quaternion.identity);

        playersHUD = goHUD.GetComponent<GUIHUD>();
        roomShaker = goRoom.GetComponent<Shaker>();
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

        changePositionReady = new bool[numberOfPlayers];
        isEverythingDisplayed = new bool[numberOfPlayers];
        BrickManager.Instance.currentBricksOnLayer = new int[numberOfPlayers];
        playersParents = new Parenting[numberOfPlayers];
        playersShakers = new Shakers[numberOfPlayers];
        firstSetUpDone = new bool[numberOfPlayers];
        ScoreManager.Instance.displayedScore = new GUIScoreData[numberOfPlayers];
        ScoreManager.Instance.displayedCombo = new GUIComboData[numberOfPlayers];
        ScoreManager.Instance.score = new float[numberOfPlayers];
        ScoreManager.Instance.combo = new float[numberOfPlayers];
        ScoreManager.Instance.brickCounterGauge = new int[numberOfPlayers];

        InitRoom();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            currentLayer[i] = -1;
            isThereAnotherLayer[i] = true;
            startPos[i] = posDiffPerPlayer * i;
            playersParents[i].layersParent = new Transform[currentLevel.level.levelWallBuilds.walls.Length];


            Vector3 goPos = new Vector3(startPos4Player1.x + (posDiffPerPlayer.x * i), startPos4Player1.y + (posDiffPerPlayer.y * i), startPos4Player1.z + (posDiffPerPlayer.z * i));
            GameObject trans = new GameObject();
            trans.transform.position = goPos;
            trans.name = "Wall_Of_Player_" + i;
            levelTrans[i] = trans.transform;

            ScoreManager.Instance.displayedScore[i] = playersHUD.ScoreData[i];
            ScoreManager.Instance.displayedCombo[i] = playersHUD.ComboData[i];



                playersShakers[i].layersShaker = new Shaker[playersParents[i].layersParent.Length];

            //Spawn les PARENTS pour chaque LAYER du mur d'un joueur
            for (int j = 0; j < playersParents[i].layersParent.Length; j++)
            {
                GameObject obj = new GameObject();
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


    /// <summary>
    /// Set up parameters to change level position
    /// </summary>
    public void SetNextLayer(int playerID)
    {
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
                    if (playerID != i)
                    {
                        if (currentLayer[playerID] < currentLayer[i])
                        {
                            rewardModifier++;
                        }
                    }
                }

                ScoreManager.Instance.SetScore((int)((float)ScoreManager.Instance.finishingFirstScoreBoost / rewardModifier), playerID);
            }

            StartCoroutine(GoWALLgO(playerID));

            if (!isEverythingDisplayed[playerID] && firstSetUpDone[playerID])
            {
                BrickManager.Instance.SpawnLayer(playerID, numberOfLayerToDisplay - 1);
            }
        }


        if (currentLayer[playerID] >= currentLevelConfig.levelWallBuilds.walls.Length - 1)
        {
            isThereAnotherLayer[playerID] = false;
        }

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
                if (obj.GetComponent<BrickBehaviours>().isMoving)
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

    IEnumerator GoWALLgO(int playerID)
    {
        yield return new WaitForSeconds(1);

        changePositionReady[playerID] = true;

        for (int i = currentLayer[playerID]; i < (currentLayer[playerID] + numberOfLayerToDisplay - 1); i++)
        {
            if (i <= currentLevel.level.levelWallBuilds.walls.Length - 1)
            {
                SetWaypoints(playerID, i);
            }
        }

        BrickManager.Instance.SetCurrentBrickOnLayer(playerID);
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
}
