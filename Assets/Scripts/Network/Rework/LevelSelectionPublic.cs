using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class LevelSelectionPublic : MonoBehaviourPunCallbacks
{
    [SerializeField] int currentLevel;
    public PhotonView pV;

    public void SetLevel(int level)
    {
        currentLevel = level;
        pV.RPC("SelectLevel", RpcTarget.AllBuffered, currentLevel);
    }

    [PunRPC]
    public void SelectLevel(int number)
    {
        MultiLevel.Instance.levelIndex = number;
        InRoomPublic.Instance.roomName.text = PhotonNetwork.CurrentRoom.Name + " Level " +  MultiLevel.Instance.levelIndex;
    }
}
