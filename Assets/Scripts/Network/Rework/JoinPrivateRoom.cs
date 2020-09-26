using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinPrivateRoom : MonoBehaviour
{
    public TextMeshProUGUI secretCodeText;

    public void JoinRoomClick()
    {
        PhotonNetwork.JoinRoom(secretCodeText.text);
    }
}
