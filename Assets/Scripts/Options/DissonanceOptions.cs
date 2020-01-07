using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;

public class DissonanceOptions : MonoBehaviour
{
    public GameObject dissonanceSetup;

    DissonanceComms dissonanceComms;

    VoiceBroadcastTrigger dissonanceVolume;

    // Start is called before the first frame update
    void Start()
    {
        dissonanceComms = dissonanceSetup.GetComponent<DissonanceComms>();
        dissonanceVolume = dissonanceSetup.GetComponent<VoiceBroadcastTrigger>();

        ModifyVolume();
    }

    void Update(){
       
    }

    void MuteMic(){
        if(dissonanceComms.IsMuted){
            dissonanceComms.IsMuted = false;
        } else {
            dissonanceComms.IsMuted = true;
        } 
    }

    void ModifyVolume(){
     //   RoomChannel channel = dissonanceComms.RoomChannels.Open("Room", amplitudeMultiplier: 1f);
     //   channel.Volume = 0.5f;
     //    Debug.Log(channel.Volume);
    }   
}
