using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;
using Dissonance.Audio;

public class DissonanceOptions : MonoBehaviour
{
    public GameObject dissonanceSetup;
    float volume;

    DissonanceComms dissonanceComms;

    VoiceBroadcastTrigger dissonanceVolume;
    OpenChannelVolumeDuck duckVolume;

    RoomChannel channel;
    VoicePlayerState voicePlayer;

    // Start is called before the first frame update
    void Start()
    {
        dissonanceComms = dissonanceSetup.GetComponent<DissonanceComms>();
        dissonanceVolume = dissonanceSetup.GetComponent<VoiceBroadcastTrigger>();

        channel = dissonanceComms.RoomChannels.Open("Room", amplitudeMultiplier: 0f);

        MuteMic();
    }

    void Update(){
       channel.Volume = volume;
    }

    void MuteMic(){
        if(dissonanceComms.IsMuted){
            dissonanceComms.IsMuted = false;
        } else {
            dissonanceComms.IsMuted = true;
        } 
    }

    public void ModifyVolume(float newVolume){
        volume = newVolume;
        Debug.Log(newVolume);
    }   
}
