using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayAgain_MultiButton_Config : MonoBehaviour
{
    [SerializeField] Button button;
    private bool launchingGame = false;


    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            button.gameObject.SetActive(false);

        button.onClick.AddListener(() => RestartGame());
        launchingGame = false;
    }

    private void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 2 && !launchingGame)
        {
            Debug.Log("MASTER CLIENT _ Restart Game");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(1);
            launchingGame = true;
        }
    }
}
