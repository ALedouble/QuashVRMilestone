using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayAgain_MultiButton_Config : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    [SerializeField] Button button;
    private bool launchingGame = false;
    public PhotonView pV;


    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            button.gameObject.SetActive(false);

        button.onClick.AddListener(() => RestartGame());
        launchingGame = false;
    }

    public void RestartGame()
    {
        pV.RPC("LoadGame", RpcTarget.All);
    }

    [PunRPC]
    private void LoadGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 2 && !launchingGame)
        {
            Debug.Log("MASTER CLIENT _ Restart Game");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(1);
            launchingGame = true;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RestartGame();
            Debug.Log("Restart");
        }
    }
}
