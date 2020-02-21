using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundTag { Brick, Impact, Wall, Floor, Racket, FrontWall, None }

[System.Serializable]
public class SoundPool
{
    public string soundPoolName;
    public SoundTag tag;
    public float cooldown;

    public Sound[] sounds;
    
    private float lastPlayTime = 0;

    public float NextPlayableTime => lastPlayTime + cooldown;

    public void PlayRandomSound(AudioSource audioSource, float soundIntensity)
    {
        Sound selectedSound = SelectRandomSound();

        SetAudioSource(audioSource, selectedSound);
        AdjustVolume(audioSource, selectedSound, soundIntensity);

        audioSource.Play();

        lastPlayTime = Time.time;
    }

    private void SetAudioSource(AudioSource source, Sound sound)
    {
        source.clip = sound.clip;
        source.volume = sound.defaultVolume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.spatialBlend = sound.spatialBlend;
        source.panStereo = sound.panStereo;
    }

    private void AdjustVolume(AudioSource audioSource, Sound sound, float soundIntensity)           // A amelioré (ajout d'un default volume?)
    {
        if(soundIntensity >= 0)
        {
            audioSource.volume *= soundIntensity;

            if (audioSource.volume < sound.minVolume)
                audioSource.volume = sound.minVolume;
            else if (audioSource.volume > sound.maxVolume)
                audioSource.volume = sound.maxVolume;
        }
    }

    private Sound SelectRandomSound()
    {
        return sounds[Random.Range(0, sounds.Length)];
    }
}
