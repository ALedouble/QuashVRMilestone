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

    bool muteByOther;

    public void SelfMute()
    {
        if (muteToggle.isOn && !muteByOther)
        {
            comms.enabled = false;
            SaveOptionMicro.Instance.enabledComms = false;
        }
        else if(!muteOther.isOn && !muteByOther)
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
            muteByOther = true;
            SaveOptionMicro.Instance.isMuted = comms.IsMuted;
        }
    }

    public void ChangeVolume()
    {
        broadcast.ActivationFader.Volume = sliderVolume.value;
        SaveOptionMicro.Instance.volumeValue = broadcast.ActivationFader.Volume;
    }

    public void Update()
    {
        SaveOptionMicro.Instance.volumeValue = broadcast.ActivationFader.Volume;
    }
}
