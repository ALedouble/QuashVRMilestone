using Dissonance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class VoiceOptionInGame : MonoBehaviour
{
    public DissonanceComms comms;
    public VoicePlayerState state;
    public VoiceBroadcastTrigger broadcast;

    // Start is called before the first frame update
    void Start()
    {
        comms.enabled = SaveOptionMicro.Instance.enabledComms;

        if (comms.enabled)
        {
            comms.IsMuted = SaveOptionMicro.Instance.isMuted;
        }

        comms.RemoteVoiceVolume = SaveOptionMicro.Instance.volumeValue;
    }
}
