using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyPublic : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static LobbyPublic Instance;

    public string roomName;
    public int roomSize = 2;

    public GameObject roomListingPrefab;
    public Transform roomsPanel;

    private void Awake()
    {
        Instance = this; // Singleton
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to master photon server.
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Photon server");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RemoveRoomListings();

        foreach(RoomInfo room in roomList)
        {
            ListRoom(room);

        }
    }

    void RemoveRoomListings()
    {
        while(roomsPanel.childCount != 0)
        {
            Destroy(roomsPanel.GetChild(0).gameObject);
        }
    }

    void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
            RoomPublic tempButton = tempListing.GetComponent<RoomPublic>();
            tempButton.SetRoom(room.Name);
        }
    }

    public void CreateRoom() // Try
    {
        Debug.Log("Trying to create room");
     //   RoomOptions roomOps = new RoomOptions() { IsVisible = true, MaxPlayers = (byte)2 };
     //   PhotonNetwork.CreateRoom("Room" + roomName, roomOps);
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
