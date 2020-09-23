using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public enum BallOwnershipSwitchType
{
    Default = 0,
    Return = 1,
    BallLoss = 2
}

public class BallMultiplayerBehaviour : MonoBehaviour
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
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        photonView = GetComponent<PhotonView>();
    }

    public void HandOverBallOwnership(BallOwnershipSwitchType switchMotive)
    {
        photonView.RPC("BecomeBallOwner", RpcTarget.Others, switchMotive);
    }

    [PunRPC]
    public void BecomeBallOwner(BallOwnershipSwitchType switchMotive)
    {
        photonView.RequestOwnership();

        if(switchMotive == BallOwnershipSwitchType.Return)
        {
            ReturnSwitchActions();
        }

        OnBallOwnershipAcquisition();
        SendLoseBallOwnershipRPC();
    }

    public void UpdateBallOwnershipAssociatedActions()
    {
        if (IsBallOwner)
            OnBallOwnershipAcquisition();
        else
            OnBallOwnershipLoss();
    }

    private void SendLoseBallOwnershipRPC()
    {
        photonView.RPC("LoseBallOwnership", RpcTarget.Others);
    }

    [PunRPC]
    private void LoseBallOwnership()
    {
        OnBallOwnershipLoss();  //Desactivé la collision avec la racket
    }
}
