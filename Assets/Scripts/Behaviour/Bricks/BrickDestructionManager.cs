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

    public List<int>[] currentLayerBricks;
    //A Déplacer dans son propre scripte
    public void HitBrickByID(int brickID, int playerID)
    {
        //Systeme de Memoire
        if ((brickID < BrickManager.Instance.AllBricks.Count && brickID >= 0) && BrickManager.Instance.CurrentLayersBricks[playerID].Contains(brickID)) //+Check que la balle soit sur le layer actif appartenant au player
        {
            Debug.Log("Brick To Be Destroyed!");
            BrickManager.Instance.AllBricks[brickID].GetComponent<BrickDestruction>().DestroyBrick();
            BrickManager.Instance.CurrentLayersBricks[playerID].Remove(brickID);
        }
        else
            Debug.LogError("Is not on CurrentLayer");
    }

    public void HitBricksByID(int[] brickIDs, int playerID)
    {
        foreach(int brickID in brickIDs)
        {
            HitBrickByID(brickID, playerID);
        }
    }
}
