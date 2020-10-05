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

    public void HitBricksByID(BrickInfo[] brickInfos, int playerID, int explosionColorID)
    {
        List<int> brickToDestroy = new List<int>();

        foreach (BrickInfo brickInfo in brickInfos)
        {
            if(ShouldBrickBeDestroyed(brickInfo, explosionColorID))
                brickToDestroy.Add(brickInfo.BrickID);
        }

        if (brickToDestroy.Count != 0)
        {
            if (GameManager.Instance.offlineMode)
                DestroyBricks(brickToDestroy.ToArray(), playerID);
            else
                photonView.RPC("DestroyBricks", RpcTarget.All, brickToDestroy.ToArray(), playerID);
        }
    }

    private bool ShouldBrickBeDestroyed(BrickInfo brickInfo, int explosionColorID)
    {
        if(brickInfo.PlayerID == (int)QPlayerManager.instance.LocalPlayerID && BrickManager.Instance.CurrentLayersBricks[brickInfo.PlayerID].Contains(brickInfo.BrickID))
        {
            return (brickInfo.colorID == 0 || brickInfo.colorID == explosionColorID);
        }

        return false;
    }

    [PunRPC]
    private void DestroyBricks(int[] brickIDs, int playerID)
    {
        bool hasBrickBeenDestroyed = false;
        foreach(int brickID in brickIDs)
        {
            BrickManager.Instance.AllBricks[playerID][brickID].GetComponent<BrickDestruction>().DestroyBrick();
            BrickManager.Instance.RemoveDestroyedBrick(brickID, playerID);
        }

        if(hasBrickBeenDestroyed)
            LevelManager.instance.ShakeLayer(playerID);
    }
}
