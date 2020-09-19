using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BrickDestructionManager : MonoBehaviour
{
    public static BrickDestructionManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public bool HitBrickByID(int brickID, int playerID)
    {
        if ((brickID < BrickManager.Instance.AllBricks.Count && brickID >= 0) && BrickManager.Instance.CurrentLayersBricks[playerID].Contains(brickID))
        {
            if(BrickManager.Instance.AllBricks[brickID].GetComponent<BrickInfo>().colorID == 0 || BrickManager.Instance.AllBricks[brickID].GetComponent<BrickInfo>().colorID == BallManager.instance.GetBallColorID())
            {
                BrickManager.Instance.AllBricks[brickID].GetComponent<BrickDestruction>().DestroyBrick();
                BrickManager.Instance.CurrentLayersBricks[playerID].Remove(brickID);

                return true;
            }
        }

        return false;
    }

    public void HitBricksByID(int[] brickIDs, int playerID)
    {
        bool hasBrickBeenDestroyed = false;
        foreach(int brickID in brickIDs)
        {
            if (HitBrickByID(brickID, playerID))
                hasBrickBeenDestroyed = true;
        }

        if(hasBrickBeenDestroyed)
            LevelManager.instance.ShakeLayer(playerID);
    }
}
