using System.Collections;
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

    public GameObject lobbyGo;
    public GameObject roomGo;
    public Transform playersPanel;
    public GameObject playerListingPrefab;
    public GameObject startButton;

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

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
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
        if (PhotonNetwork.InRoom)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                GameObject tempList = Instantiate(playerListingPrefab, playersPanel);
                TextMeshProUGUI tempText = tempList.transform.GetComponent<TextMeshProUGUI>();
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
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        playersInRoom--;
        ClearPlayerListings();
        ListPlayers();
    }
}
