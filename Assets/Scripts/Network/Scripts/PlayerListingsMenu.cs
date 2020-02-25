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
        photonView = GetComponent<PhotonView>();
        GetCurrentRoomPlayers();
    }
    #endregion

     [SerializeField]
    private Transform content;
    [SerializeField]
    private PlayerListing _playerListing;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomCanvasGroup _roomsCanvases;

    public GameObject levelSelectionCanvas;
    public GameObject buttonLaunch;

    [SerializeField]
    public int numLevel;

    PhotonView photonView;


    private void Start() {

    }

    private void Update() {

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("hello");
            buttonLaunch.SetActive(true);
        }
        else
        {
            buttonLaunch.SetActive(false);
        }

        if (_listings.Count == 1){
            levelSelectionCanvas.SetActive(true);
        }
        else{
            levelSelectionCanvas.SetActive(false);
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
        if(PhotonNetwork.IsMasterClient && _listings.Count == 1){
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible= false;
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void SetLevel(Toggle toggle){
        if (toggle.name == "Level 01"){
            numLevel = 0;
            photonView.RPC("SelectLevel", RpcTarget.All, numLevel);
        } else if (toggle.name == "Level 02"){
            numLevel = 1;
            photonView.RPC("SelectLevel", RpcTarget.All, numLevel);
        } else if (toggle.name == "Level 03"){
            numLevel = 2;
            photonView.RPC("SelectLevel", RpcTarget.All, numLevel);
        } else if (toggle.name == "Level 04"){
            numLevel = 3;
            photonView.RPC("SelectLevel", RpcTarget.All, numLevel);
        }
    }

    [PunRPC]
    public void SelectLevel(int number){
        MultiLevel.Instance.levelIndex = number;
    }

    public void KickPlayer(Player player){
        PhotonNetwork.CloseConnection(player);
    }
}

