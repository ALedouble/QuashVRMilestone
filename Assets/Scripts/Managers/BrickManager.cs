using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrickManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public WallBuilds levelWallsConfig;

    [Header("Number of bricks on the current layer")]
    public int totalBricskOnLayer;
    public int currentBricksOnLayer;

    [Header("Number of bricks in the level")]
    public int totalBricskInLevel;

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
    }

    void UpdateBrickLevel()
    {
        currentBricksOnLayer -= 1;

        if(currentBricksOnLayer <= 0)
        {
            LevelManager.Instance.SetNextLayer();
        }
    }

}
