using Dissonance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OptionVoice : MonoBehaviour
{
    public DissonanceComms comms;
    public VoiceBroadcastTrigger broadcast;
    public Toggle muteToggle;
    public Toggle muteOther;
    public Slider sliderVolume;

    public PhotonView pV;

    public void SelfMute()
    {
        if (muteToggle.isOn)
        {
            comms.enabled = false;
            SaveOptionMicro.Instance.enabledComms = false;
        }
        else
        {
            comms.enabled = true;
            SaveOptionMicro.Instance.enabledComms = true;
        }
    }

    public void MuteOther()
    {
        if (muteOther.isOn)
        {
            pV.RPC("MuteOtherPlayer", RpcTarget.Others, true);
        }
        else
        {
            pV.RPC("MuteOtherPlayer", RpcTarget.Others, false);
        }
    }

    [PunRPC] 
    public void MuteOtherPlayer(bool value)
    {
        if (comms.enabled)
        {
            comms.IsMuted = value;
            SaveOptionMicro.Instance.isMuted = comms.IsMuted;
        }
    }

    public void ChangeVolume()
    {
        broadcast.ActivationFader.Volume = sliderVolume.value;
        SaveOptionMicro.Instance.volumeValue = broadcast.ActivationFader.Volume;
    }
}
