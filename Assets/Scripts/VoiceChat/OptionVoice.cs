using Dissonance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionVoice : MonoBehaviour
{
    public GameObject dissonance;
    DissonanceComms comms;
    VoiceBroadcastTrigger vbt;

    private void Awake()
    {
        comms = dissonance.GetComponent<DissonanceComms>();
        vbt = dissonance.GetComponent<VoiceBroadcastTrigger>();

        IsMuted();
    }

    private void Update()
    {
        Debug.Log(comms.IsMuted);
    }

    private void IsMuted()
    {
        comms.IsMuted = true;
    }

    private void SoundVolume()
    {
        comms.RemoteVoiceVolume = 0.5f;
    }
}
