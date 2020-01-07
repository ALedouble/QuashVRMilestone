using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
  [SerializeField]
  private Text _text;

  GameObject currentRoom;
  GameObject joinRoom;

  public RoomInfo RoomInfo{ get; private set; }

    private void Start()
    {
        currentRoom = GameObject.FindGameObjectWithTag("currentroom");
        joinRoom = GameObject.FindGameObjectWithTag("joinroom");

        Debug.Log(joinRoom);
    }

    public void SetRoomInfo(RoomInfo roomInfo){
      RoomInfo = roomInfo;
      _text.text = roomInfo.MaxPlayers + ", " + roomInfo.Name;
  }

  public void OnClick_Button(){
       
        joinRoom.SetActive(false);
        currentRoom.SetActive(true);
        PhotonNetwork.JoinRoom(RoomInfo.Name);
   
  }
}
