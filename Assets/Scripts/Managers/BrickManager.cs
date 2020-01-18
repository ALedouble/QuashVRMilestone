using System.Collections;
using Photon.Pun;
using Photon;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrickManager : MonoBehaviourPunCallbacks
{
    [Header("Récupération de la configuration du level")]
    public WallBuilds levelWallsConfig = new WallBuilds();
    public GameObject prefabBase;
    [HideInInspector] public string prefabPath = "Assets/Prefabs/Bricks";

    public PresetScriptable[] colorPresets;
    [HideInInspector] public string presetPath = "Assets/ScriptableObjects/ColorPresets";

    public BrickTypesScriptable[] brickPresets;
    [HideInInspector] public string brickPresetPath = "Assets/ScriptableObjects/BrickPresets";

    [Header("Number of bricks on the current layer")]
    public int[] currentBricksOnLayer;
    public float offsetPerPlayer;

    [Header("Bonus & Malus settings")]
    [SerializeField] int bonusPoolID;
    [SerializeField] int malusPoolID;

    [Header("Shaking")]
    public Shake layerShake;
    public Shaker roomShaker;

    [Header("Audio")]
    public string soundName;
    [SerializeField] float hitIntensity;

    public static BrickManager Instance;




    private void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// Détruit la brique
    /// </summary>
    /// <param name="brickToDestroy">Brick that will be detroyed</param>
    /// <param name="brickValue">Brick value for the score</param>
    public void DeadBrick(BrickInfo touchedBrick)
    {
        Vector3 brickPos = touchedBrick.Transform.position;
        touchedBrick.Transform.gameObject.SetActive(false);
        touchedBrick.Transform.parent = null;

        Vector3 cross = Vector3.Cross(touchedBrick.Transform.up, touchedBrick.Transform.right);

        PoolManager.instance.SpawnFromPool("CubeImpactFX", brickPos, Quaternion.LookRotation(cross, Vector3.up));
        PoolManager.instance.SpawnFromPool("CubeDeathFX", brickPos, Quaternion.LookRotation(cross, Vector3.up));


        GameObject score = PoolManager.instance.SpawnFromPool("ScoreText", brickPos, Quaternion.identity);

        float newScore = touchedBrick.ScoreValue * ScoreManager.Instance.combo[(int)BallManager.instance.GetLastPlayerWhoHitTheBall()]; //BallID

        score.GetComponent<HitScoreBehaviour>().SetHitValues(newScore, colorPresets[0].colorPresets[touchedBrick.ColorID].coreEmissiveColors);

        LevelManager levelManager = LevelManager.instance;
        levelManager.playersShakers[touchedBrick.WallID].layersShaker[levelManager.currentLayer[touchedBrick.WallID]].PlayShake(layerShake);
        roomShaker?.PlayShake();

        //Bonus & malus case
        if (touchedBrick.IsBonus) BonusManager.instance.SpawnRandomObject(touchedBrick.Transform);
        if (touchedBrick.IsMalus) MalusManager.instance.SpawnRandomObject(touchedBrick.Transform);

        
        if (!GameManager.Instance.offlineMode){
            ScoreManager.Instance.pV.RPC("SetScore", RpcTarget.All, touchedBrick.ScoreValue, (int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
            ScoreManager.Instance.pV.RPC("SetCombo", RpcTarget.All, (int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
        }
        else{
            ScoreManager.Instance.SetScore(touchedBrick.ScoreValue, (int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
            ScoreManager.Instance.SetCombo((int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
        }
       
        ScoreManager.Instance.resetCombo = false;
        UpdateBrickLevel(touchedBrick.WallID);

        //AudioManager.instance.PlayHitSound(soundName, touchedBrick.Transform.position, touchedBrick.Transform.rotation, hitIntensity);
    }


    /// <summary>
    /// Check the number of Bricks on the current layer and call the next one when it hits 0
    /// </summary>
    /// <param name="playerID"></param>
    void UpdateBrickLevel(int playerID)
    {
        SetCurrentBrickOnLayer(playerID);

        if (currentBricksOnLayer[playerID] <= 0)
        {
            LevelManager.instance.SetNextLayer(playerID);
        }
    }

    /// <summary>
    /// Spawn a layer
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="currentDisplacement"></param>
    public void SpawnLayer(int playerID, int currentDisplacement)
    {
        Wall layerToSpawn = levelWallsConfig.walls[LevelManager.instance.currentLayer[playerID] + currentDisplacement];

        for (int i = 0; i < layerToSpawn.wallBricks.Count; i++)
        {
            if (layerToSpawn.wallBricks[i].isBrickHere)
            {
                Vector3 brickNewPos = new Vector3(layerToSpawn.wallBricks[i].brickPosition.x, layerToSpawn.wallBricks[i].brickPosition.y,
                    (layerToSpawn.wallBricks[i].brickPosition.z + (LevelManager.instance.layerDiffPosition * ((float)LevelManager.instance.currentLayer[playerID] + (float)currentDisplacement))));


                GameObject obj = PoolManager.instance.SpawnFromPool("Brick", LevelManager.instance.levelTrans[playerID].position, Quaternion.identity);
                BrickBehaviours objBehaviours = obj.GetComponent<BrickBehaviours>();
                MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
                Material[] mats = objMesh.sharedMaterials;

                mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
                mats[1].SetFloat("_Metallic", 0.75f);

                mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
                mats[0].SetColor("_FresnelColor", colorPresets[0].colorPresets[layerToSpawn.wallBricks[i].brickColorPreset].fresnelColors);
                mats[0].SetColor("_CoreEmissiveColor", colorPresets[0].colorPresets[layerToSpawn.wallBricks[i].brickColorPreset].coreEmissiveColors);
                mats[0].SetFloat("_XFrameThickness", 0.75f);
                mats[0].SetFloat("_YFrameThickness", 0.75f);

                objMesh.sharedMaterials = mats;



                obj.transform.parent = LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID] + currentDisplacement];

                obj.name = layerToSpawn.wallBricks[i].brickID;

                obj.transform.localPosition = brickNewPos;



                objBehaviours.armorPoints = brickPresets[0].brickPresets[layerToSpawn.wallBricks[i].brickTypePreset].armorValue;
                objBehaviours.scoreValue = brickPresets[0].brickPresets[layerToSpawn.wallBricks[i].brickTypePreset].scoreValue;

                objBehaviours.isBonus = layerToSpawn.wallBricks[i].isBonus;
                objBehaviours.isMalus = layerToSpawn.wallBricks[i].isMalus;

                objBehaviours.colorID = layerToSpawn.wallBricks[i].brickColorPreset;
                objBehaviours.wallID = playerID;
                objBehaviours.savedInIndex = i;

                if (layerToSpawn.wallBricks[i].isMoving)
                {
                    //objBehaviours.isMoving = layerToSpawn.wallBricks[i].isMoving;// Related to the Level Manager NOW
                    objBehaviours.speed = layerToSpawn.wallBricks[i].speed;
                    objBehaviours.smoothTime = layerToSpawn.wallBricks[i].smoothTime;
                    objBehaviours.waypoints = new List<Vector3>();

                    for (int j = 0; j < layerToSpawn.wallBricks[i].waypointsStorage.Count; j++)
                    {
                        Vector3 waypointToLayer = new Vector3(layerToSpawn.wallBricks[i].waypointsStorage[j].x, layerToSpawn.wallBricks[i].waypointsStorage[j].y,
                            layerToSpawn.wallBricks[i].waypointsStorage[j].z + (LevelManager.instance.layerDiffPosition * ((float)LevelManager.instance.currentLayer[playerID] + (float)currentDisplacement)));

                        objBehaviours.waypoints.Add(waypointToLayer);
                    }
                }
            }
        }


        if (LevelManager.instance.currentLayer[playerID] + currentDisplacement >= levelWallsConfig.walls.Length - 1)
        {
            LevelManager.instance.isEverythingDisplayed[playerID] = true;
        }
    }

    /// <summary>
    /// Activate bricks movement on the current front layer
    /// </summary>
    /// <param name="bricksTransform"></param>
    /// <param name="playerID"></param>
    public void ActivateMovingBricks(int playerID)
    {
        Wall layerToSpawn = levelWallsConfig.walls[LevelManager.instance.currentLayer[playerID]];

        List<BrickBehaviours> bricks = new List<BrickBehaviours>();

        //Debug.Log("Current layer : " + LevelManager.instance.currentLayer[playerID]);
        //Debug.Log("Child Count : " + LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID]].childCount);
        if (LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID]].childCount > 0)
        {
            for (int i = 0; i < LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID]].childCount; i++)
            {
                bricks.Add(LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID]].GetChild(i).gameObject.GetComponent<BrickBehaviours>());
            }
        }


        for (int i = 0; i < bricks.Count; i++)
        {
            if (layerToSpawn.wallBricks[bricks[i].savedInIndex].isMoving)
            {
                bricks[i].isMoving = true;
            }
        }
    }

    /// <summary>
    /// Get the number of Bricks on the current layer
    /// </summary>
    /// <param name="playerID"></param>
    public void SetCurrentBrickOnLayer(int playerID)
    {
        currentBricksOnLayer[playerID] = LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID]].childCount;
    }
}
