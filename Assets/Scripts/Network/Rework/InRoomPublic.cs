﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class InRoomPublic : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static InRoomPublic Instance;
    private PhotonView photonView;

    //Player info
    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public bool isGameLoaded;
    public int currentScene;

    public TextMeshProUGUI roomName;
    public GameObject lobbyGo;
    public GameObject roomGo;
    public GameObject levelSelectionGo;
    public GameObject roomSelectionGo;
    public Transform playersPanel;
    public GameObject playerListingPrefab;
    public GameObject startButton;
    public GameObject kickButton;

    public PhotonView pV;

    bool launchingGame;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            if(Instance != this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        lobbyGo.SetActive(false);
        roomGo.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
            kickButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
            kickButton.SetActive(false);
        }

        ClearPlayerListings();

        ListPlayers();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;


    }

    void ClearPlayerListings()
    {
        for(int i = playersPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(playersPanel.GetChild(i).gameObject);
        }
    }

    void ListPlayers()
    {
        Debug.Log("list");
        if (PhotonNetwork.InRoom)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                Debug.Log("instantiate");
                GameObject tempList = Instantiate(playerListingPrefab, playersPanel);
                TextMeshProUGUI tempText = tempList.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                tempText.text = player.NickName;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        ClearPlayerListings();
        ListPlayers();
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        Debug.Log("entered");

        if(playersInRoom == 2)
        {

        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);



        playersInRoom--;
        ClearPlayerListings();
        ListPlayers();
    }

    public void LeaveRoomClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pV.RPC("Kick", RpcTarget.Others);
            PhotonNetwork.CurrentRoom.RemovedFromList = true;
            roomGo.SetActive(false);
            roomSelectionGo.SetActive(true);
            PhotonNetwork.LeaveRoom(true);

        }
        else
        {
            roomSelectionGo.SetActive(true);
            roomGo.SetActive(false);
            PhotonNetwork.LeaveRoom(true);
        }
    }

    [PunRPC]
    public void Kick()
    {
        if (PhotonNetwork.InRoom)
        {
            roomSelectionGo.SetActive(true);
            roomGo.SetActive(false);
            PhotonNetwork.LeaveRoom(true);
        }
    }

    public void KickPlayer()
    {
        pV.RPC("Kick", RpcTarget.Others);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && playersInRoom == 2 && !launchingGame)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
            launchingGame = true;
        }
    }
}