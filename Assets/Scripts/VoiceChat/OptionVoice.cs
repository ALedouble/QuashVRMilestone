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

    bool isMuted;

    private void Awake()
    {
        comms = dissonance.GetComponent<DissonanceComms>();
        vbt = dissonance.GetComponent<VoiceBroadcastTrigger>();
        photonView = GetComponent<PhotonView>();

        if (isMuted == true)
        {
            comms.IsMuted = true;
        }
    }

    

    private void Update()
    {
        
    }

    public void IsMuted()
    {
        comms.IsMuted = muteToggle.isOn;
        isMuted = muteToggle;
    }

    public void Toggle_MuteOtherPlayer(){
        photonView.RPC("MuteOtherPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void MuteOtherPlayer(){
        comms.IsMuted = true;
    }
}
