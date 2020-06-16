using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;


public class CreateRoom : MonoBehaviourPunCallbacks
{
    public static CreateRoom Instance;
    [SerializeField]
    [HideInInspector]
    public TextMeshProUGUI _roomName;
    private RoomCanvasGroup roomCanvases;
    
    public Button buttonCreate;

    void Awake(){
        Instance = this;
    }

    private void Start() {
    }

    public void FirstInitialize(RoomCanvasGroup canvases){
        roomCanvases = canvases;
    }

    public void OnClick_CreateRoom(){
        if(!PhotonNetwork.IsConnected){
            return;
        }
            

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(PhotonNetwork.NickName, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom(){
      Debug.Log("Created room successfully.");
      roomCanvases.CurrentRoomCanvas.Show();
      roomCanvases.CreateOrJoinRoomCanvas.Hide();
      PlayerListingsMenu.Instance.GetCurrentRoomPlayers();
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        Debug.Log("Room creation failed:" + message, this);
    }

    IEnumerator GetCurrentRoom()
    {
        yield return new WaitForSeconds(1f);
       
        Debug.Log("checking success");
    }
}
