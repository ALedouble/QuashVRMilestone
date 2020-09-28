using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum BallOwnershipSwitchType
{
    Default = 0,
    Return = 1,
    BallLoss = 2
}

public class BallMultiplayerBehaviour : MonoBehaviour, IPunOwnershipCallbacks
{
    public static BallMultiplayerBehaviour Instance { get; private set; }
    public bool IsBallOwner { get => photonView.IsMine; }

    public delegate void OnBallMultiplayerEvent();
    public event OnBallMultiplayerEvent OnBallOwnershipAcquisition;
    public event OnBallMultiplayerEvent ReturnSwitchActions;
    public event OnBallMultiplayerEvent OnBallOwnershipLoss;

    private PhotonView photonView;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        photonView = GetComponent<PhotonView>();

        PhotonNetwork.AddCallbackTarget(this);

        OnBallOwnershipAcquisition += ExitReturnCase;
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    

    public void HandOverBallOwnership(BallOwnershipSwitchType switchMotive)
    {
        Debug.Log("Hand over ball ownership");
        photonView.RPC("BecomeBallOwner", RpcTarget.Others, switchMotive);
    }

    [PunRPC]
    public void BecomeBallOwner(BallOwnershipSwitchType switchMotive)
    {
        if (switchMotive == BallOwnershipSwitchType.Return)
            OnBallOwnershipAcquisition += ReturnSwitchActions;

        photonView.RequestOwnership();
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (photonView == targetView)
            photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if(photonView == targetView)
            UpdateBallOwnershipBasedStates();
    }

    public void UpdateBallOwnershipBasedStates()
    {
        if (IsBallOwner)
            OnBallOwnershipAcquisition?.Invoke();
        else
            OnBallOwnershipLoss?.Invoke();
    }

    [PunRPC]
    private void AquireBallOwnership()
    {
        OnBallOwnershipAcquisition?.Invoke();
    }

    [PunRPC]
    private void LoseBallOwnership()
    {
        OnBallOwnershipLoss?.Invoke();  //Desactivé la collision avec la racket
        Debug.Log("LoseBallOwnerShip");
    }

    private void ExitReturnCase()
    {
        OnBallOwnershipAcquisition -= ReturnSwitchActions;
    }
}
