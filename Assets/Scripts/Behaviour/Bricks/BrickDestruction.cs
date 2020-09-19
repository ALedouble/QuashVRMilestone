using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BrickDestruction : MonoBehaviour
{
    private BrickInfo brickInfo;

    private void Awake()
    {
        brickInfo = GetComponent<BrickInfo>();
    }

    /// <summary>
    /// Détruit la brique, augmente le score, renvoie les feedbacks et spawn les bonus/malus
    public void DestroyBrick()
    {
        ScorePoints();

        DespawnBrick();

        SendBreakFeedbacks();

        DropBonusMalus();

        //AudioManager.instance.PlayHitSound(soundName, touchedBrick.Transform.position, touchedBrick.Transform.rotation, hitIntensity);
    }

    private void DespawnBrick()
    {
        gameObject.SetActive(false);
        transform.parent = null;
        BrickManager.Instance.UpdateBrickLevel(brickInfo.wallID);
    }

    private void SendBreakFeedbacks()
    {
        /// SFX
        AudioManager.instance.PlaySound("BrickExplosion", Vector3.zero);

        /// FX
        switch (brickInfo.colorID)
        {
            case 0:
                PoolManager.instance.SpawnFromPool("Brick_Destroyed_White", transform.position + new Vector3(0, 0, -0.5f), Quaternion.LookRotation(transform.forward, Vector3.up));
                break;

            case 1:
                PoolManager.instance.SpawnFromPool("Brick_Destroyed_Blue", transform.position + new Vector3(0, 0, -0.5f), Quaternion.LookRotation(transform.forward, Vector3.up));
                break;

            case 2:
                PoolManager.instance.SpawnFromPool("Brick_Destroyed_Green", transform.position + new Vector3(0, 0, -0.5f), Quaternion.LookRotation(transform.forward, Vector3.up));
                break;
        }

        ///// Skake
        //LevelManager.instance.ShakeLayer(brickInfo.wallID);
    }

    private void ScorePoints()
    {
        /// Score

        ScoreManager.Instance.BuildScoreText(brickInfo.scoreValue, brickInfo.colorID, transform.position, transform.rotation);


        ScoreManager.Instance.SetScore(brickInfo.scoreValue, (int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
        ScoreManager.Instance.SetCombo((int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID


        ScoreManager.Instance.resetCombo = false;
    }

    private void DropBonusMalus()
    {
        //Bonus & malus case
        if (brickInfo.isBonus) BonusManager.instance.SpawnRandomObject(transform);
        if (brickInfo.isMalus) MalusManager.instance.SpawnRandomObject(transform);
    }
}
