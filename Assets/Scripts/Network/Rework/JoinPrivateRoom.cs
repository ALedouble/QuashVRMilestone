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
        for (int i = 0; i < LobbyPublic.Instance.roomListings.Count; i++)
        {
            if(LobbyPublic.Instance.roomListings[i].Name == secretCodeText.text)
            {
                PhotonNetwork.JoinRoom(secretCodeText.text);
            }
        }
    }
}
