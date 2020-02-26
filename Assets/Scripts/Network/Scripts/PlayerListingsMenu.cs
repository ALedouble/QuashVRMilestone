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
    public GameObject kickPlayerButton;

    public GameObject mainScreen;
    public GameObject currentRoom;

    [SerializeField]
    public int numLevel;

    PhotonView photonView;


    private void Start() {
        StartCoroutine("checkCurrentRoom");
    }

    private void Update() {

        
        if (PhotonNetwork.IsMasterClient)
        {
            buttonLaunch.SetActive(true);
        }
        else
        {
            buttonLaunch.SetActive(false);
            levelSelectionCanvas.SetActive(false);
            kickPlayerButton.SetActive(false);
        }

        if (_listings.Count == 2 && PhotonNetwork.IsMasterClient){
            levelSelectionCanvas.SetActive(true);
            kickPlayerButton.SetActive(true);
        }
        else{
            levelSelectionCanvas.SetActive(false);
        }
    }

    IEnumerator checkCurrentRoom(){
        yield return new WaitForSeconds(0.5f);
        GetCurrentRoomPlayers();
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
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
       AddPlayerListing(newPlayer);
      // checkCurrentRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1){
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public void OnClick_StartGame(){
        if(PhotonNetwork.IsMasterClient && _listings.Count == 2){
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

    public void OnClick_KickPlayer(){
        OnPlayerLeftRoom(PhotonNetwork.PlayerListOthers[0]);
        photonView.RPC("ResetPlayerToMainScreen", RpcTarget.Others);
    }

    public void KickPlayer(Player player){
        
       // PhotonNetwork.CloseConnection(player);
    }

    [PunRPC]
    public void ResetPlayerToMainScreen(){
        if(!PhotonNetwork.IsMasterClient){
            mainScreen.SetActive(true);
            currentRoom.SetActive(false);
        }
    }
}

