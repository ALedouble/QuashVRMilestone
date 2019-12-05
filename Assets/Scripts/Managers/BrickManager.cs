using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrickManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public WallBuilds levelWallsConfig;
    public GameObject prefabBase;
    public string prefabPath = "Assets/Prefabs/Bricks";

    public PresetScriptable[] colorPresets;
    public string presetPath = "Assets/ScriptableObjects/ColorPresets";

    public BrickTypesScriptable[] brickPresets;
    public string brickPresetPath = "Assets/ScriptableObjects/BrickPresets";

    [Header("Number of bricks on the current layer")]
    public int[] currentBricksOnLayer;
    public float offsetPerPlayer;

    /*
    [Header("Number of bricks in the level")]
    public int totalBricskInLevel;
    */

    [Header("Bonus & Malus settings")]
    [SerializeField] int bonusPoolID;
    [SerializeField] int malusPoolID;




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

        PoolManager.instance.SpawnFromPool("CubeImpactFX", brickPos, Quaternion.identity);
        ScoreManager.Instance.IncrementScore(touchedBrick.ScoreValue);

        //Bonus & malus case
        if (touchedBrick.IsBonus) BonusManager.instance.SpawnRandomObject(touchedBrick.Transform);
        if (touchedBrick.IsMalus) MalusManager.instance.SpawnRandomObject(touchedBrick.Transform);

        UpdateBrickLevel(touchedBrick.WallID);
    }

    void UpdateBrickLevel(int playerID)
    {
        currentBricksOnLayer[playerID]--;

        if (currentBricksOnLayer[playerID] <= 0)
        {
            LevelManager.Instance.SetNextLayer(playerID);
        }
    }

    public void SpawnLayer(int playerID)
    {
        Wall layerToSpawn = levelWallsConfig.walls[LevelManager.Instance.currentLayer[playerID]];

        Debug.Log("Spawn check");

        for (int i = 0; i < layerToSpawn.wallBricks.Count; i++)
        {
            if (layerToSpawn.wallBricks[i].isBrickHere)
            {
                Debug.Log("layerToSpawn.wallBricks.Count : " + layerToSpawn.wallBricks.Count);

                Vector3 brickNewPos = new Vector3(layerToSpawn.wallBricks[i].brickPosition.x, 
                    layerToSpawn.wallBricks[i].brickPosition.y,
                    (LevelManager.Instance.startPos[playerID].z + (LevelManager.Instance.layerDiffPosition * (float)LevelManager.Instance.currentLayer[playerID])));

                //GameObject obj = Instantiate(prefabBase) as GameObject;
                GameObject obj = PoolManager.instance.SpawnFromPool("Brick", LevelManager.Instance.levelTrans[playerID].position, Quaternion.identity);
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



                obj.transform.parent = LevelManager.Instance.levelTrans[playerID];

                obj.name = layerToSpawn.wallBricks[i].brickID;

                obj.transform.localPosition = new Vector3(layerToSpawn.wallBricks[i].brickPosition.x, layerToSpawn.wallBricks[i].brickPosition.y,
                    (layerToSpawn.wallBricks[i].brickPosition.z + (LevelManager.Instance.layerDiffPosition * (float)LevelManager.Instance.currentLayer[playerID])));



                objBehaviours.armorPoints = layerToSpawn.wallBricks[i].armorValue;
                objBehaviours.scoreValue = layerToSpawn.wallBricks[i].scoreValue;

                objBehaviours.isBonus = layerToSpawn.wallBricks[i].isBonus;
                objBehaviours.isMalus = layerToSpawn.wallBricks[i].isMalus;

                objBehaviours.colorID = layerToSpawn.wallBricks[i].brickColorPreset;
                objBehaviours.wallID = playerID;

                if (layerToSpawn.wallBricks[i].isMoving)
                {
                    objBehaviours.isMoving = layerToSpawn.wallBricks[i].isMoving;
                    objBehaviours.speed = layerToSpawn.wallBricks[i].speed;
                    objBehaviours.smoothTime = layerToSpawn.wallBricks[i].smoothTime;
                    objBehaviours.waypoints = new List<Vector3>();

                    for (int j = 0; j < layerToSpawn.wallBricks[i].waypointsStorage.Count; j++)
                    {
                        objBehaviours.waypoints.Add(layerToSpawn.wallBricks[i].waypointsStorage[j]);
                    }
                }


                currentBricksOnLayer[playerID]++;
            }
        }
    }
}
