using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static PlayerListingsMenu Instance;

    void Awake()
    {
        Instance = this;
        GetCurrentRoomPlayers();
    }
    #endregion
     [SerializeField]
    private Transform content;
    [SerializeField]
    private PlayerListing _playerListing;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomCanvasGroup _roomsCanvases;

    [SerializeField]
    public int numLevel;

    void Update()
    {
        if(Input.GetKey(KeyCode.P)){
            PlayerPrefs.SetInt("level", numLevel);
            PhotonNetwork.LoadLevel(1);           
        }  
    }

    public void FirstInitialize(RoomCanvasGroup canvases){
        _roomsCanvases = canvases;
    }

    public override void OnLeftRoom(){
        content.DestroyChildren();
    }

    private void GetCurrentRoomPlayers(){
        if(!PhotonNetwork.IsConnected)
            return;

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        
        foreach(KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListing(playerInfo.Value);
        }        
    }

    private void AddPlayerListing(Player player){
        PlayerListing listing = Instantiate(_playerListing, content);
        
            if (listing != null){
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }

        Debug.Log(listing);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
       AddPlayerListing(newPlayer);
       Debug.Log(_listings);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1){
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public void OnClick_StartGame(){
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible= false;
            PhotonNetwork.LoadLevel(1);
        }
    }

    void SetLevel(Button button){
        if (button.name == "Level"){
            numLevel = 0;
        } else if (button.name == "Level (1)"){
            numLevel = 1;
        } else if (button.name == "Level (2)"){
            numLevel = 2;
        } else if (button.name == "Level (3)"){
            numLevel = 3;
        }
    }
}

