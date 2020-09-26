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

    public List<RoomInfo> roomListings;

    public List<GameObject> listRoomGo;

    private int numberRoom;

    private void Awake()
    {
        Instance = this; // Singleton
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to master photon server.
        roomListings = new List<RoomInfo>();
        listRoomGo = new List<GameObject>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Photon server");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
      //  RemoveRoomListings();

        int tempIndex;
        foreach(RoomInfo room in roomList)
        {
            if(roomListings != null)
            {
                tempIndex = roomListings.FindIndex(ByName(room.Name));
            }
            else
            {
                tempIndex = -1;
            }

            if(tempIndex != -1)
            {
                roomListings.RemoveAt(tempIndex);
                Destroy(roomsPanel.GetChild(tempIndex).gameObject);
            }
            else
            {
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void RemoveRoomListings()
    {
        int i = 0;
        while(roomsPanel.childCount != 0)
        {
            Destroy(roomsPanel.GetChild(i).gameObject);
            i++;
        }
    }

    void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
            Room tempButton = tempListing.GetComponent<Room>();
            tempButton.roomName = room.Name;
            tempButton.SetRoom();
            listRoomGo.Add(tempListing);
        }
    }

    public void CreateRoom() // Try to create room
    {
        Debug.Log("Trying to create room");

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, MaxPlayers = (byte)2 };
        PhotonNetwork.CreateRoom(GetRoomName(), roomOps);
    }

    public void CreatePrivateRoom()
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = false, MaxPlayers = (byte)2 };
        PhotonNetwork.CreateRoom(GetPrivateCode(), roomOps);
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
            Debug.Log("refresh"); 
        }
    }

    public string GetRoomName()
    {
        
        roomName = "Room " + Random.Range(0, 1000).ToString();

        
        return roomName;
    }

    public string GetPrivateCode()
    {
        int newNumber = Random.Range(10000, 99999);

        return newNumber.ToString();
    }
}
