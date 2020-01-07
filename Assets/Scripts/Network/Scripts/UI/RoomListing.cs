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

  GameObject mainCanvas;
  GameObject joinRoom;
   Transform currentRoom;

  public RoomInfo RoomInfo{ get; private set; }

    private void Start()
    {
        mainCanvas = GameObject.FindGameObjectWithTag("maincanvas");
        currentRoom = mainCanvas.transform.Find("■■■■ Current Room Screen ■■■■");
        

        joinRoom = GameObject.FindGameObjectWithTag("joinroom");
    }

    public void SetRoomInfo(RoomInfo roomInfo){
      RoomInfo = roomInfo;
      _text.text = roomInfo.MaxPlayers + ", " + roomInfo.Name;
  }

  public void OnClick_Button(){
       
        joinRoom.SetActive(false);
        currentRoom.gameObject.SetActive(true);
        currentRoom.GetChild(0).gameObject.SetActive(true);
        currentRoom.GetChild(1).gameObject.SetActive(true);
        currentRoom.GetChild(3).gameObject.SetActive(true);


        PhotonNetwork.JoinRoom(RoomInfo.Name);
   
  }
}
