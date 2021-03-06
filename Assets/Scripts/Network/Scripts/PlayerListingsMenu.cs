﻿using Photon.Pun;
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
    }
    #endregion

     [SerializeField]
    private Transform content;
    [SerializeField]
    public PlayerListing _playerListing;

    public List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomCanvasGroup _roomsCanvases;

    public GameObject levelSelectionCanvas;
    public GameObject buttonLaunch;
    public GameObject kickPlayerButton;

    public GameObject mainScreen;
    public GameObject currentRoom;

    [SerializeField]
    public int numLevel;

    PhotonView photonView;
    bool launchingGame = false;

    private Coroutine checkCurrentRoomCoroutine;
    public override void OnEnable() {

        checkCurrentRoomCoroutine = StartCoroutine(CheckCurrentRoom());
    }

    public override void OnDisable()
    {
        base.OnDisable();

        StopCoroutine(checkCurrentRoomCoroutine);

        for(int i = 0; i < _listings.Count; i++)
        {
            Destroy(_listings[i].gameObject);
        }

        _listings.Clear();
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

    IEnumerator CheckCurrentRoom(){
        
        yield return new WaitForSeconds(0.5f);
        Debug.Log("test coroutine");
        GetCurrentRoomPlayers();
    }

    public void FirstInitialize(RoomCanvasGroup canvases){
        _roomsCanvases = canvases;
    }



    public void GetCurrentRoomPlayers(){
        if(!PhotonNetwork.IsConnected)
            return;

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        
        foreach(KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListing(playerInfo.Value);
        }        
    }

    private void AddPlayerListing(Player player){
        int index = _listings.FindIndex(x => x.Player == player);
        if(index != -1)
        {
            Debug.Log("index not - 1");
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, content);
            Debug.Log("index - 1");
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        Debug.Log("entered in room");
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
        if(PhotonNetwork.IsMasterClient && _listings.Count == 2 && !launchingGame){
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible= false;
            PhotonNetwork.LoadLevel(1);
            launchingGame = true;
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
        Player player = PhotonNetwork.PlayerList[1];
        OnPlayerLeftRoom(player);
        PhotonNetwork.CloseConnection(player); 
        photonView.RPC("ResetPlayerToMainScreen", RpcTarget.Others);
    }

    public void OnClick_BackToMenu(){
        if(PhotonNetwork.IsMasterClient){
            OnPlayerLeftRoom(PhotonNetwork.PlayerListOthers[0]);
            photonView.RPC("ResetPlayerToMainScreen", RpcTarget.Others);
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.LeaveRoom();
           // OnPlayerLeftRoom(PhotonNetwork.PlayerList[1]);
        }
    }

    public void KickPlayer(Player player){
        
       // PhotonNetwork.CloseConnection(player);
    }

    [PunRPC]
    public void ResetPlayerToMainScreen(){
        if(!PhotonNetwork.IsMasterClient){
            PhotonNetwork.LeaveRoom();
            _listings = new List<PlayerListing>();
            mainScreen.SetActive(true);
            currentRoom.SetActive(false);
           // PhotonNetwork.Disconnect();

        }

        //Instantiate(GameManager.Instance.warningPrefab, GameManager.Instance.warningTransform);
    }
}

