using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using VRTK;

public enum PlayerID 
{
    NONE = -1,
    PLAYER1 = 0,
    PLAYER2 = 1
}

public enum PlayerHand
{
    RIGHT = 2,
    LEFT = 1
}

public class QPlayerManager : MonoBehaviourPun
{
    #region Singleton
    public static QPlayerManager instance;

    private void Awake()
    {
        //if(instance)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        instance = this;
    }
    #endregion

    public GameObject localPlayer;

    private GameObject localPlayerRightController;
    private GameObject localPlayerLeftController;

    public QPlayer LocalPlayerID
    {
        get
        {
            if (GameManager.Instance.offlineMode)
                return QPlayer.PLAYER1;
            else if (PhotonNetwork.IsMasterClient)
                return QPlayer.PLAYER1;
            else
                return QPlayer.PLAYER2;
        }
    }

    public void SetLocalPlayer(GameObject localPlayer)
    {
        this.localPlayer = localPlayer;
        SetupControllers();
    }

    public GameObject GetLocalController(PlayerHand playerHand)
    {
        if(playerHand == PlayerHand.LEFT)
        {
            return localPlayerLeftController;
        }
        else
        {
            return localPlayerRightController;
        }
    }

    private void SetupControllers()
    {
        localPlayerLeftController = localPlayer.GetComponentInChildren<LeftControllerGetter>().Get();
        localPlayerRightController = localPlayer.GetComponentInChildren<RightControllerGetter>().Get();

        PlayerInputManager.instance.SetupInputMod();
    }
}
