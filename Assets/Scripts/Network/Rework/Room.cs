using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Room : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public string roomName;

    public void SetRoom()
    {
        nameText.text = roomName;
    }

    public void JoinRoomClick()
    {
        PhotonNetwork.JoinRoom(roomName);

        LobbyPublic.Instance.roomListings.Clear();
        for(int i = 0; i < LobbyPublic.Instance.listRoomGo.Count; i++)
        {
            Destroy(LobbyPublic.Instance.listRoomGo[i]);
        }
    }
}
