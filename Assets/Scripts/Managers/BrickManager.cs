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

    //BrickInfo currentBrickInfo;

    private PhotonView photonView;
    private List<GameObject> AllBricks;



    private void Awake()
    {
        Instance = this;
        AllBricks = new List<GameObject>();

        photonView = GetComponent<PhotonView>();
    }

    public void AddBrick(GameObject newBrick)
    {
        if(newBrick.GetComponent<BrickBehaviours>().BrickID != AllBricks.Count)
        {
            Debug.Log("Bad BrickID");
        }
        AllBricks.Add(newBrick);

    }

    /// <summary>
    /// Check the number of Bricks on the current layer and call the next one when it hits 0
    /// </summary>
    /// <param name="playerID"></param>
    public void UpdateBrickLevel(int playerID)
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
                BrickInfo brickInfo = obj.GetComponent<BrickInfo>();
                MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
                Material[] mats = objMesh.sharedMaterials;

                mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
                mats[1].SetFloat("_Metallic", 0.75f);

                mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
                mats[0].SetColor("_FresnelColor", LevelManager.instance.colorPresets[0].colorPresets[layerToSpawn.wallBricks[i].brickColorPreset].fresnelColors);
                mats[0].SetColor("_CoreEmissiveColor", LevelManager.instance.colorPresets[0].colorPresets[layerToSpawn.wallBricks[i].brickColorPreset].coreEmissiveColors);
                mats[0].SetFloat("_XFrameThickness", 0.75f);
                mats[0].SetFloat("_YFrameThickness", 0.75f);

                objMesh.sharedMaterials = mats;



                obj.transform.parent = LevelManager.instance.playersParents[playerID].layersParent[LevelManager.instance.currentLayer[playerID] + currentDisplacement];

                obj.name = layerToSpawn.wallBricks[i].brickID;

                obj.transform.localPosition = brickNewPos;



                //brickInfo.armorPoints = brickPresets[0].brickPresets[layerToSpawn.wallBricks[i].brickTypePreset].armorValue;
                brickInfo.armorValue = brickPresets[0].brickPresets[layerToSpawn.wallBricks[i].brickTypePreset].armorValue;
                brickInfo.scoreValue = brickPresets[0].brickPresets[layerToSpawn.wallBricks[i].brickTypePreset].scoreValue;

                brickInfo.isBonus = layerToSpawn.wallBricks[i].isBonus;
                brickInfo.isMalus = layerToSpawn.wallBricks[i].isMalus;

                brickInfo.colorID = layerToSpawn.wallBricks[i].brickColorPreset;
                brickInfo.wallID = playerID;
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

    public void DestroyBrickByID(int brickID)
    {
        Debug.Log("DestroyBrickByID");
        if(brickID < AllBricks.Count && brickID >= 0)
        {
            if (PhotonNetwork.OfflineMode)
            {
                AllBricks[brickID].GetComponent<BrickBehaviours>().DestroyBrick();
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("DestroyBrickByIDRPC", RpcTarget.All, brickID);
            }
        }
    }

    [PunRPC]
    private void DestroyBrickByIDRPC(int brickID)
    {
        AllBricks[brickID].GetComponent<BrickBehaviours>().DestroyBrick();
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


    public void ScorePoints(BrickInfo brickInfo)
    {
        /// Score
        Debug.Log("ScorePoints");
        ScoreManager.Instance.BuildScoreText(brickInfo.scoreValue, brickInfo.colorID, transform.position, transform.rotation);

        if (!GameManager.Instance.offlineMode)
        {
            photonView.RPC("SetScoreAndComboRPC", RpcTarget.All, brickInfo.scoreValue, (int)BallManager.instance.GetLastPlayerWhoHitTheBall()); 
        }
        else
        {
            ScoreManager.Instance.SetScore(brickInfo.scoreValue, (int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
            ScoreManager.Instance.SetCombo((int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
        }

        ScoreManager.Instance.resetCombo = false;
    }

    [PunRPC]
    private void SetScoreAndComboRPC(int scoreValue, int playerID)
    {
        Debug.Log("ScorePointsRPC");
        ScoreManager.Instance.SetScore(scoreValue, playerID);
        ScoreManager.Instance.SetCombo(playerID);
    }
}
