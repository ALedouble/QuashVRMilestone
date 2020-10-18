using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionPublic : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    int currentLevel;
    public PhotonView photonView;

    public void SetLevel(int level)
    {
        currentLevel = level;
        photonView.RPC("SelectLevel", RpcTarget.All, currentLevel);
    }

    public void RestartLevel()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            PhotonNetwork.LoadLevel(1);
    }

    [PunRPC]
    public void SelectLevel(int number)
    {
        MultiLevel.Instance.levelIndex = number;
    }

    public void DisconnectPlayer()
    {
        StartCoroutine("DisconnectAndLoad");
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.InRoom)
            yield return null;

        SceneManager.LoadScene("Scene_Menu");
    }
}
