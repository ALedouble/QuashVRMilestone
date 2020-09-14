using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RoomPublic : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public void SetRoom(string roomName)
    {
        nameText.text = roomName;
    }

    public void JoinRoomClick()
    {
        PhotonNetwork.JoinRoom(nameText.text);
    }
}
