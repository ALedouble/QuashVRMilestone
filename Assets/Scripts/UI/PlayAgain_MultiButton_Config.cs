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


    int currentLevel;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            button.gameObject.SetActive(false);

        button.onClick.AddListener(() => GameManager.Instance.RestartMultiplayerGame());
        launchingGame = false;
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
        photonView.RPC("SelectLevel", RpcTarget.All, currentLevel);
    }

    public void SelectLevel(int number)
    {
        MultiLevel.Instance.levelIndex = number;
    }
}
