﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSettings : MonoBehaviour
{
    [System.Serializable]
    private class PlayerPreferences
    {
        public PlayerHand dominantHand;
        //public float shoulderHeight;

        public SwitchColorInputType switchColorInputType;

        [Range(1f, 2f)]
        public float flashIntensity;

        public bool hadDominantHandWarning;

        public PlayerPreferences(PlayerHand dominantHand, /* float shoulderHeight, */SwitchColorInputType switchColorInputType, float flashIntensity)
        {
            this.dominantHand = dominantHand;
            //this.shoulderHeight = shoulderHeight;
            this.switchColorInputType = switchColorInputType;
            this.flashIntensity = flashIntensity;
            hadDominantHandWarning = false;
        }
    }


    public static PlayerSettings Instance;

    #region Settings Min Max Default Values

    [Header("DominantHand Settings")]
    public PlayerHand dominantHandDefaultValue = PlayerHand.RIGHT;

    //[Header("ShoulderHeight Settings")]
    //public float shoulderHeightMin;
    //public float shoulderHeightMax;
    //public float shoulderHeightDefaultValue;

    [Header("SwitchColorInputType Settings")]
    public SwitchColorInputType switchColorInputTypeDefaultValue = SwitchColorInputType.Hold;

    [Header("FlashIntensity Settings")]
    [Range(1f, 2f)]
    public float flashIntensityDefaultValue = 2;

    #endregion

    public PlayerHand PlayerDominantHand
    {
        get => playerPreferences.dominantHand;
        set
        {
            if (IsDominantHandValid(value))
            {
                playerPreferences.dominantHand = value;
                SavePlayerSettings();
            }
        }
    }

    #region ShoulderHeight
    //public float PlayerShoulderHeight 
    //{
    //    get => playerPreferences.dominantHand;
    //    set
    //    {
    //        if(playerPreferences.shoulderHeight < -2f || playerPreferences.shoulderHeight > 4f)
    //        {
    //            playerPreferences.shoulderHeight = value;
    //            SavePlayerSettings();
    //        }
    //    }
    //}
    #endregion

    public SwitchColorInputType SwitchColorInputType
    {
        get => playerPreferences.switchColorInputType;
        set
        {
            if (IsSwitchColorInputTypeValid(value))
            {
                playerPreferences.switchColorInputType = value;
                SavePlayerSettings();
            }
        }
    }

    public float FlashIntensity
    {
        get => playerPreferences.flashIntensity - 1f;
        set
        {
            if (IsFlashIntensityValid(value + 1f))
            {
                playerPreferences.flashIntensity = value + 1f;
                Debug.Log("Flash Intensity : " + playerPreferences.flashIntensity);
                SavePlayerSettings();
            }
            else
                Debug.Log("Wrong Flash Intensity : " + playerPreferences.flashIntensity);
        }
    }

    public bool HadDominantHandWarning
    {
        get => playerPreferences.hadDominantHandWarning;
        set
        {
            playerPreferences.hadDominantHandWarning = value;
            SavePlayerSettings();
        }
    }

    private string fileName = "/PlayerSettings.json";
    private PlayerPreferences playerPreferences;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPlayerSettings();
    }

    public void LoadPlayerSettings()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + fileName))
        {
            string preferencesToLoad = System.IO.File.ReadAllText(Application.persistentDataPath + fileName);
            playerPreferences = JsonUtility.FromJson<PlayerPreferences>(preferencesToLoad);
            if (!CheckPreferencesIntegrity(playerPreferences))
                SavePlayerSettings();
        }
        else
        {
            playerPreferences = CreateNewPlayerPreferences();
            SavePlayerSettings();
        }
    }

    public void SavePlayerSettings()
    {
        string preferencesToSave = JsonUtility.ToJson(playerPreferences);
        System.IO.File.WriteAllText(Application.persistentDataPath + fileName, preferencesToSave);
    }

    private PlayerPreferences CreateNewPlayerPreferences()
    {
        PlayerPreferences newDefaultPlayerSettings = new PlayerPreferences(dominantHandDefaultValue,/* shoulderHeightDefaultValue,*/ switchColorInputTypeDefaultValue, flashIntensityDefaultValue);

        return newDefaultPlayerSettings;
    }

    private bool CheckPreferencesIntegrity(PlayerPreferences preferrences)
    {
        bool preferencesWasValid = true;

        if (!IsDominantHandValid(preferrences.dominantHand))
        {
            preferrences.dominantHand = dominantHandDefaultValue;
            preferencesWasValid = false;
        }

        //if(!IsShoulderHeightValid(preferences.shoulderHeight))
        //{
        //    preferences.shoulderHeight = shoulderHeightDefaultValue;
        //    preferenceWasValid = false;
        //}

        if (!IsSwitchColorInputTypeValid(preferrences.switchColorInputType))
        {
            preferrences.switchColorInputType = switchColorInputTypeDefaultValue;
            preferencesWasValid = false;
        }

        if (!IsFlashIntensityValid(preferrences.flashIntensity))
        {
            preferrences.flashIntensity = flashIntensityDefaultValue;
            preferencesWasValid = false;
        }

        return preferencesWasValid;
    }

    #region Validity Methods

    private bool IsPreferenceValid(PlayerPreferences preferences)
    {
        bool isValid = true;

        if (!IsDominantHandValid(preferences.dominantHand))
            isValid = false;

        //if (!IsShoulderHeightValid(preferences.shoulderHeight))
        //    isValid = false;

        if (!IsSwitchColorInputTypeValid(preferences.switchColorInputType))
            isValid = false;

        if (!IsFlashIntensityValid(preferences.flashIntensity))
            isValid = false;

        return isValid;
    }

    private bool IsDominantHandValid(PlayerHand value)
    {
        return value == PlayerHand.LEFT || value == PlayerHand.RIGHT;
    }

    //private bool IsShoulderHeightValid(float value)
    //{
    //    return value >= shoulderHeightMin && value <= shoulderHeightMax;
    //}

    private bool IsSwitchColorInputTypeValid(SwitchColorInputType value)
    {
        return value == SwitchColorInputType.Hold || value == SwitchColorInputType.Click;
    }

    private bool IsFlashIntensityValid(float value)
    {
        return value >= 1f && value <= 2f;
    }

    #endregion
}
