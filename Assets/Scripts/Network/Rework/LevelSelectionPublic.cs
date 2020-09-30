using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionPublic : MonoBehaviour
{
    int currentLevel;
    public PhotonView photonView;

    public void SetLevel(int level)
    {
        currentLevel = level;
        photonView.RPC("SelectLevel", RpcTarget.All, currentLevel);
    }

    [PunRPC]
    public void SelectLevel(int number)
    {
        MultiLevel.Instance.levelIndex = number;
    }
}
