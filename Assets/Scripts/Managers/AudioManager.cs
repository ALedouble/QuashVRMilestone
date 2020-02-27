using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Malee;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public SoundPreset soundPreset;

    private Dictionary<string, SoundPool> soundDictionary;

    private void Awake()
    {
        instance = this;

        InitSoundDictionary();
    }

    private void InitSoundDictionary()
    {
        soundDictionary = new Dictionary<string, SoundPool>();

        foreach(SoundPool soundPool in soundPreset.soundPools)
        {
            soundPool.Initialize();
            string soundTag = soundPool.soundPoolName;
            if (!soundDictionary.ContainsKey(soundTag))
            {
                soundDictionary.Add(soundTag, soundPool);

                //Debug.Log("SoundPool " + soundTag + " add to soundPool dictionary");
            }
            else
            {
                Debug.LogError("SoundPoolKey " + soundTag + " already Exist!");
            }
        }
    }

    public void PlaySound(string soundPoolName, Vector3 spawnPosition, float soundIntensity = 1)                 // A modifier pour permettre passage RPC. Attention RandomSound pour réseau...?
    {
        //Debug.Log("Play sound querry for " + soundPoolName);

        if (soundDictionary.ContainsKey(soundPoolName))
        {
            if (Time.time < soundDictionary[soundPoolName].NextPlayableTime)
            {
                //Debug.Log("SoundPool " + soundPoolName + " is on cooldown!");
                return;
            }

            GameObject audioSourceGameObject = (GameObject)PoolManager.instance?.SpawnFromPool("AudioSource", spawnPosition, Quaternion.identity);
            AudioSource audioSource = audioSourceGameObject?.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                soundDictionary[soundPoolName].PlayRandomSound(audioSource, soundIntensity);
            }
        }
        else
        {
            Debug.LogError("Sound tag " + soundPoolName + " has no SoundPool associated with!");
        }
    }
}
