using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Photon.Pun;

public class BrickDestructionManager : MonoBehaviour
{
    public static BrickDestructionManager Instance;

    private PhotonView photonView;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        photonView = GetComponent<PhotonView>();
    }

    public void HitBricksByID(int[] brickIDs, int playerID)
    {
        List<int> brickToDestroy = new List<int>();

        foreach (int brickID in brickIDs)
        {
            if(ShouldBrickBeDestroyed(brickID, playerID))
                brickToDestroy.Add(brickID);
        }

        if (brickToDestroy.Count != 0)
        {
            if (GameManager.Instance.offlineMode)
                DestroyBricks(brickToDestroy.ToArray(), playerID);
            else
                photonView.RPC("DestroyBricks", RpcTarget.Others, brickToDestroy.ToArray(), playerID);
        }
            
    }

    private bool ShouldBrickBeDestroyed(int brickID, int playerID)
    {
        if (BrickManager.Instance.CurrentLayersBricks[playerID].Contains(brickID))
        {
            return true;
        }
        return false;
    }

    [PunRPC]
    private void DestroyBricks(int[] brickIDs, int playerID)
    {
        foreach(int brickID in brickIDs)
        {
            BrickManager.Instance.AllBricks[brickID].GetComponent<BrickDestruction>().DestroyBrick();
            BrickManager.Instance.CurrentLayersBricks[playerID].Remove(brickID);
        }
    }
}
