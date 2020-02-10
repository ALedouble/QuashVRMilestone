using Dissonance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionVoice : MonoBehaviour
{
    public GameObject dissonance;
    DissonanceComms comms;
    VoiceBroadcastTrigger vbt;
    public Toggle muteToggle;

    private void Awake()
    {
        comms = dissonance.GetComponent<DissonanceComms>();
        vbt = dissonance.GetComponent<VoiceBroadcastTrigger>();
        IsMuted();
    }

    private void Update()
    {
        
    }

    public void IsMuted()
    {
        comms.IsMuted = muteToggle.isOn;
    }


}
