using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyPublic : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static LobbyPublic Instance;

    private string roomName;
    public int roomSize = 2;

    public GameObject roomListingPrefab;
    public Transform roomsPanel;

    private int numberRoom;

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
            numberRoom = roomList.Count;
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
            tempButton.roomName = room.Name;
            tempButton.SetRoom();
        }
    }

    public void CreateRoom() // Try to create room
    {
        Debug.Log("Trying to create room");

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, MaxPlayers = (byte)2 };
        PhotonNetwork.CreateRoom(GetRoomName(), roomOps);
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public string GetRoomName()
    {
        roomName = "Room " + (PhotonNetwork.CountOfRooms + 1).ToString();
        return roomName;
    }
}
