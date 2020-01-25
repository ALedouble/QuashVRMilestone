﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Malee;

public class AudioManager : MonoBehaviour
{
    #region SingletonPart
    public static AudioManager instance;


    private void Awake()
    {
        instance = this;
    }
    #endregion




    public SoundClass soundList;
    private SoundSettings selectedSound;

    public void PlayHitSound(string tag, Vector3 spawnPosition, Quaternion spawnRotation, float hitIntensity)
    {

        bool hasSoundAssociatedWith = false;

        for (int i = 0; i < soundList.sounds.Length; i++)
        {
            if (tag == soundList.sounds[i].tag.ToString())
            {
                selectedSound = soundList.sounds[i];
                hasSoundAssociatedWith = true;
                break;
            }

        }

        if (!hasSoundAssociatedWith)
        {
            return;
        }

        if (selectedSound.clip == null)
        {
            Debug.LogWarning("SOUND NOT FOUND");
            return;
        }

        if (Time.time < selectedSound.lastPlayTime + selectedSound.cooldown)
        {
            return;
        }

        GameObject hitSoundGameObject = (GameObject)PoolManager.instance?.SpawnFromPool("AudioSource", spawnPosition, spawnRotation);
        AudioSource hitSoundSource = hitSoundGameObject.GetComponent<AudioSource>();

        SetAudioSource(hitSoundSource, selectedSound);
        AdjustVolume(hitSoundSource, selectedSound, hitIntensity);

        /*
        if (tag == "Racket")
        {
            VibrationManager.instance.VibrateOn(hitSoundSource.clip);
        }
        */

        hitSoundSource.Play();
    }

    public void PlaySound(string soundName, Vector3 soundPosition)
    {
        for (int i = 0; i < soundList.sounds.Length; i++)
        {
            if (soundName == soundList.sounds[i].soundName)
            {
                selectedSound = soundList.sounds[i];
                break;
            }
        }

        if (selectedSound.clip == null)
        {
            Debug.LogWarning("SOUND NOT FOUND");
            return;
        }

        if (Time.time < selectedSound.lastPlayTime + selectedSound.cooldown)
        {
            return;
        }

        GameObject hitSoundGameObject = (GameObject)PoolManager.instance?.SpawnFromPool("AudioSource", Vector3.zero, Quaternion.identity);
        AudioSource hitSoundSource = hitSoundGameObject.GetComponent<AudioSource>();

        SetAudioSource(hitSoundSource, selectedSound);
        AdjustVolume(hitSoundSource, selectedSound, selectedSound.volume);

        hitSoundSource.Play();
    }

    public void PlayRacketSound(string soundName, Vector3 soundPosition)
    {
        for (int i = 0; i < soundList.sounds.Length; i++)
        {
            if (soundName == soundList.sounds[i].soundName)
            {
                selectedSound = soundList.sounds[i];
                break;
            }
        }

        if (selectedSound.clip == null)
        {
            Debug.LogWarning("SOUND NOT FOUND");
            return;
        }

        if (Time.time < selectedSound.lastPlayTime + selectedSound.cooldown)
        {
            return;
        }

        GameObject hitSoundGameObject = (GameObject)PoolManager.instance?.SpawnFromPool("AudioSource", Vector3.zero, Quaternion.identity);
        AudioSource hitSoundSource = hitSoundGameObject.GetComponent<AudioSource>();

        SetAudioSource(hitSoundSource, selectedSound);
        AdjustVolume(hitSoundSource, selectedSound, selectedSound.volume);

        if(RacketManager.instance.isEmpowered == true)
        {
            hitSoundSource.Play();
        }
        else
        {
            hitSoundSource.Stop();
        }

    }


    private void SetAudioSource(AudioSource source, SoundSettings sound)
    {
        source.clip = sound.clip;
        //source.outputAudioMixerGroup = sound.output;          //util?
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.spatialBlend = sound.spatialBlend;
        source.panStereo = sound.panStereo;
    }

    private void AdjustVolume(AudioSource source, SoundSettings sound, float hitIntensity)          // A améliorer
    {
        source.volume *= hitIntensity / sound.maxHitMagnitude;

        if (source.volume < sound.minVolume)
            source.volume = sound.minVolume;
        else if (source.volume > sound.maxVolume)
            source.volume = sound.maxVolume;
    }
}

// TO PLAY A SOUND IN ANOTHER SCRIPT :
//
// FindObjectOfType<AudioManager>().Play("name");
// FindObjectOfType<AudioManager>().Stop("name");
//
//"name" is equal to the string of the sound you want to play

// VIEILLE METHODE
//
//public void Stop(string name)
//{
//    SoundClass s = Array.Find(sounds, sound => sound.name == name);

//    if (s == null)
//    {
//        Debug.LogWarning("SOUND NOT FOUND");
//        return;
//    }

//    s.source.Stop();
//}

//public bool isPlaying(string name)
//{
//    SoundClass s = Array.Find(sounds, sound => sound.name == name);

//    if (s == null)
//    {
//        Debug.LogWarning("SOUND NOT FOUND");
//        return false;
//    }

//    return s.source.isPlaying;
//}
