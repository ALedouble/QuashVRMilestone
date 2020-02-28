using Dissonance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OptionVoice : MonoBehaviour
{
    public GameObject dissonance;
    DissonanceComms comms;
    VoiceBroadcastTrigger vbt;
    public Toggle muteToggle;
    PhotonView photonView;

    private void Awake()
    {
        comms = dissonance.GetComponent<DissonanceComms>();
        vbt = dissonance.GetComponent<VoiceBroadcastTrigger>();
        photonView = GetComponent<PhotonView>();
    }



    private void Update()
    {
        
    }

    public void IsMuted()
    {
        comms.IsMuted = muteToggle.isOn;
    }

    public void Toggle_MuteOtherPlayer(){
        photonView.RPC("MuteOtherPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void MuteOtherPlayer(){
        comms.IsMuted = true;
    }
}
