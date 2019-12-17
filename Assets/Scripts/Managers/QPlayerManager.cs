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
    RIGHT,
    LEFT
}

public enum GrabState
{
    UNUSED,
    DELAYED,
    ATTRACTED,
    GRABBED
}

public class QPlayerManager : MonoBehaviourPun
{
    #region Singleton
    public static QPlayerManager instance;

    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(instance);
    }
    #endregion

    public GameObject localPlayer;

    private GameObject localPlayerRightController;
    private GameObject localPlayerLeftController;

    public GameObject player1 = null;
    public GameObject player2 = null;

    private GameObject player1RightController;
    private GameObject player1LeftController;

    private GameObject player2RightController;
    private GameObject player2LeftController;

    private void Start()
    {
        SetupControllers();
    }

    public void SetLocalPlayer(GameObject localPlayer)
    {
        this.localPlayer = localPlayer;
    }

    public void SetPlayer(GameObject player, int PlayerID)
    {
        if (PlayerID == 1)
        {
            player1 = player;
        }
        else if (PlayerID == 2)
        {
            player2 = player;
        }
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

    public GameObject GetController(PlayerID playerID, PlayerHand playerHand)         //A editer pour chaque cas player
    {
        if(playerID == PlayerID.PLAYER1)
        {
            if (playerHand == PlayerHand.RIGHT)
                return player1RightController;
            else
                return player1LeftController;
        }
        else if(playerID == PlayerID.PLAYER2)
        {
            if (playerHand == PlayerHand.RIGHT)
                return player2RightController;
            else
                return player2LeftController;
        }
        
        return  player1LeftController;
    }

    private void SetupControllers()
    {
        localPlayerLeftController = localPlayer.GetComponentInChildren<LeftControllerGetter>().Get();
        localPlayerRightController = localPlayer.GetComponentInChildren<RightControllerGetter>().Get();

        //player1LeftController = player1.GetComponentInChildren<LeftControllerGetter>().Get();
        //player1RightController = player1.GetComponentInChildren<RightControllerGetter>().Get();
        /*
        if (player2)
        {
            player2LeftController = player2?.GetComponentInChildren<LeftControllerGetter>().Get();
            player2RightController = player2?.GetComponentInChildren<RightControllerGetter>().Get();
        }
        */
    }

    ////////////////////////////////////    VRTKUnityEvents Called methods     /////////////////////////////////////

    public void OnPlayer1RightTriggerPress()
    {
        //ActionCall(PlayerID.PLAYER1, PlayerHand.RIGHT);
    }

    public void OnPlayer1RightTriggerRelease()
    {
        //StopCall(PlayerID.PLAYER1, PlayerHand.RIGHT);
    }

    public void OnPlayer1LeftTriggerPress()
    {
        //ActionCall(PlayerID.PLAYER1, PlayerHand.LEFT);
    }

    public void OnPlayer1LeftTriggerRelease()
    {
        //StopCall(PlayerID.PLAYER1, PlayerHand.LEFT);
    }

    public void OnPlayer2RightTriggerPress()
    {
        //ActionCall(PlayerID.PLAYER2, PlayerHand.RIGHT);
    }

    public void OnPlayer2RightTriggerRelease()
    {
        //StopCall(PlayerID.PLAYER2, PlayerHand.RIGHT);
    }

    public void OnPlayer2LeftTriggerPress()
    {
        //ActionCall(PlayerID.PLAYER2, PlayerHand.LEFT);
    }

    public void OnPlayer2LeftTriggerRelease()
    {
        //StopCall(PlayerID.PLAYER2, PlayerHand.LEFT);
    }


    private void ActionCall(PlayerID playerID, PlayerHand playerHand)           //Rename
    {
        //if(RacketManager.instance.GetGrabInfo().grabState == GrabState.GRABBED)
        //{
        //    if(RacketManager.instance.GetGrabInfo().userID == playerID)
        //    {
        //        //if (RacketManager.instance.GetGrabInfo().userHand == playerHand)
        //        //    RacketManager.instance.StopRacketGrabCall(playerID);
        //        //else
        //        //{
        //        //    if(BallManager.instance.GetGrabInfo().grabState == GrabState.UNUSED)
        //        //    {
        //        //        BallManager.instance.BallResetCall(playerID, playerHand);
        //        //    }
        //        //}
        //    }
        //    else
        //    {
        //        return; //A enlever?
        //    }
        //}
        //else if(RacketManager.instance.GetGrabInfo().grabState == GrabState.ATTRACTED)
        //{
        //    //if(RacketManager.instance.GetGrabInfo().userID == playerID)
        //        //RacketManager.instance.StopRacketGrabCall(playerID);
        //}
        //else if(RacketManager.instance.GetGrabInfo().grabState == GrabState.UNUSED)
        //{
        //    //RacketManager.instance.RacketGrabCall(playerID, playerHand);
        //}
        
        //if (BallManager.instance.GetGrabInfo().grabState == GrabState.ATTRACTED)
        //{
        //    BallManager.instance.BallResetStopCall(playerID);
        //}
        //else if (BallManager.instance.GetGrabInfo().grabState == GrabState.GRABBED)
        //{
        //    BallManager.instance.BallResetStopCall(playerID);
        //}
    }

    //private void StopCall(PlayerID playerID, PlayerHand playerHand)             //Rename
    //{
    //    if(BallManager.instance.GetGrabInfo().userID == playerID && BallManager.instance.GetGrabInfo().userHand == playerHand)
    //    {
    //        if(BallManager.instance.GetGrabInfo().grabState == GrabState.DELAYED)
    //        {
    //            BallManager.instance.BallResetStopCall(playerID);
    //        }
    //    }
    //}

    //private void ConvertInByte(byte grabByte){
    //    GrabState newGrab = (GrabState)grabByte;
    //}
    
}
