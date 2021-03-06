﻿using System.Collections;
using Photon.Pun;
using Photon;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrickManager : MonoBehaviourPunCallbacks
{
    [Header("Layer Update Duration")]
    public float layerChangeDuration = 0.90f;

    [Header("Récupération de la configuration du level")]
    public WallBuilds levelWallsConfig = new WallBuilds();
    public GameObject prefabBase;
    [HideInInspector] public string prefabPath = "Assets/Prefabs/Bricks";

    public PresetScriptable[] colorPresets;

    [HideInInspector] public string presetPath = "Assets/ScriptableObjects/ColorPresets";

    public BrickTypesScriptable[] brickPresets;
    [HideInInspector] public string brickPresetPath = "Assets/ScriptableObjects/BrickPresets";

    public int[] currentLayersBrickCount; //Bad naming...

    private List<List<int>>[] layersBricks;
    public List<int>[] CurrentLayersBricks { get; private set; }
    public Dictionary<int, GameObject>[] AllBricks { get; private set; }

    private int[] playerBrickLastID;

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

    //BrickInfo currentBrickInfo;

    private PhotonView photonView;
    
    private void Awake()
    {
        Instance = this;

        Reset();

        photonView = GetComponent<PhotonView>();
    }

    public void Reset()
    {
        DeactivateAllBricks();

        AllBricks = new Dictionary<int, GameObject>[2];
        AllBricks[0] = new Dictionary<int, GameObject>();
        AllBricks[1] = new Dictionary<int, GameObject>();

        layersBricks = new List<List<int>>[2];
        layersBricks[0] = new List<List<int>>();
        layersBricks[1] = new List<List<int>>();

        CurrentLayersBricks = new List<int>[2];
        CurrentLayersBricks[0] = new List<int>();
        CurrentLayersBricks[1] = new List<int>();

        playerBrickLastID = new int[2];
        playerBrickLastID[0] = 0;
        playerBrickLastID[1] = 0;
    }

    /// <summary>
    /// Check the number of Bricks on the current layer and call the next one when it hits 0
    /// </summary>
    /// <param name="playerID"></param>
    public void UpdateBrickLevel(int playerID)
    {
        SetCurrentBrickOnLayer(playerID);

        if (currentLayersBrickCount[playerID] <= 0)
        {
            LevelManager.instance.SetNextLayer(playerID);

            UpdateCurrentLayerWithDelay(playerID);
        }
    }

    /// <summary>
    /// Spawn a layer
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="currentDisplacement"></param>
    public void SpawnLayer(int playerID, int currentDisplacement)
    {
        //Get the wall composition reference from the correct scriptable object
        Wall layerToSpawn = levelWallsConfig.walls[LevelManager.instance.currentLayer[playerID] + currentDisplacement];
        List<int> layerBrickIDs = new List<int>();


        for (int i = 0; i < layerToSpawn.wallBricks.Count; i++)
        {
            if (layerToSpawn.wallBricks[i].isBrickHere)
            {
                

                //Activate a "Brick" gameobject from the pool and get needed components
                GameObject spawnedBrick = PoolManager.instance.SpawnFromPool("Brick", LevelManager.instance.levelTrans[playerID].position, Quaternion.identity);
                BrickBehaviours objBehaviours = spawnedBrick.GetComponent<BrickBehaviours>();
                BrickInfo brickInfo = spawnedBrick.GetComponent<BrickInfo>();
                MeshRenderer objMesh = spawnedBrick.GetComponent<MeshRenderer>();

                //Create and set a shader from colorPresets assigned
                Material[] mats = objMesh.sharedMaterials;
                mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
                mats[1].SetFloat("_Metallic", 0.75f);

                mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
                mats[0].SetColor("_FresnelColor", LevelManager.instance.colorPresets[0].colorPresets[layerToSpawn.wallBricks[i].brickColorPreset].fresnelColors);
                mats[0].SetColor("_CoreEmissiveColor", LevelManager.instance.colorPresets[0].colorPresets[layerToSpawn.wallBricks[i].brickColorPreset].coreEmissiveColors);
                mats[0].SetFloat("_XFrameThickness", 0.75f);
                mats[0].SetFloat("_YFrameThickness", 0.75f);

                //Apply shader to new brick materials
                objMesh.sharedMaterials = mats;

                //Set new brick parent and displacement (evoluting considering layers count)
                spawnedBrick.transform.parent = LevelManager.instance.playersParents[playerID].layersParent[(LevelManager.instance.currentLayer[playerID] + currentDisplacement)];

                //Set new brick name for better identification
                spawnedBrick.name = layerToSpawn.wallBricks[i].brickID;

                //Set new brick position
                Vector3 brickNewPos = new Vector3(layerToSpawn.wallBricks[i].brickPosition.x, layerToSpawn.wallBricks[i].brickPosition.y,
                    (layerToSpawn.wallBricks[i].brickPosition.z + (LevelManager.instance.layerDiffPosition * ((float)LevelManager.instance.currentLayer[playerID] + (float)currentDisplacement))));

                spawnedBrick.transform.localPosition = brickNewPos;

                // BrickID setup + Ref
                int brickID = ++playerBrickLastID[playerID];
                brickInfo.SetBrickID(brickID, playerID);
                AddBrick(spawnedBrick, brickID, playerID);
                
                //Set new brick gameplay values
                brickInfo.armorValue = brickPresets[0].brickPresets[layerToSpawn.wallBricks[i].brickTypePreset].armorValue;
                brickInfo.scoreValue = brickPresets[0].brickPresets[layerToSpawn.wallBricks[i].brickTypePreset].scoreValue;

                brickInfo.isBonus = layerToSpawn.wallBricks[i].isBonus;
                brickInfo.isMalus = layerToSpawn.wallBricks[i].isMalus;

                brickInfo.colorID = layerToSpawn.wallBricks[i].brickColorPreset;
                brickInfo.wallID = playerID;
                objBehaviours.savedInIndex = i;

                if (layerToSpawn.wallBricks[i].isMoving)
                {
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

                //Add new brick to reference list
                layerBrickIDs.Add(brickInfo.BrickID);
            }
        }

        layersBricks[playerID].Add(layerBrickIDs);

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
                bricks[i].isAMovingBrick = true;
            }
        }
    }

    /// <summary>
    /// Get the number of Bricks on the current layer
    /// </summary>
    /// <param name="playerID"></param>
    public void SetCurrentBrickOnLayer(int playerID)
    {
        currentLayersBrickCount[playerID] = LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID]].childCount;
    }

    private void UpdateCurrentLayerWithDelay(int playerID)
    {
        StartCoroutine(UpdateCurrentLayerCoroutine(playerID));
    }

    private IEnumerator UpdateCurrentLayerCoroutine(int playerID)
    {
        yield return new WaitForSeconds(layerChangeDuration);

        layersBricks[playerID].RemoveAt(0);

        if (layersBricks[playerID].Count != 0)
            SetCurrentActiveLayerBricks(playerID);
    }

    public void SetCurrentActiveLayerBricks(int playerID)
    {
        CurrentLayersBricks[playerID] = layersBricks[playerID][0];
        //Debug.Log("Player " + playerID + " active layer brick count : " + CurrentLayersBricks[playerID].Count);
    }

    private void AddBrick(GameObject newBrick, int brickID, int playerID)
    {
        AllBricks[playerID].Add(brickID, newBrick);
    }

    public void RemoveDestroyedBrick(int brickID, int playerID)
    {
        if(CurrentLayersBricks[playerID].Contains(brickID))
            CurrentLayersBricks[playerID].Remove(brickID);

        AllBricks[playerID].Remove(brickID);
    }

    private void DeactivateAllBricks()
    {
        if(AllBricks != null)
        {
            foreach (Dictionary<int, GameObject> brickList in AllBricks)
            {
                if (brickList != null)
                {
                    foreach (KeyValuePair<int, GameObject> brick in brickList)
                    {
                        brick.Value.SetActive(false);
                    }

                    brickList.Clear();
                }
            }
        }
    }
}
