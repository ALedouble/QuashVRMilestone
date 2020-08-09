using System.Collections;
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
        [Range(0f,1f)]
        public float flashIntensity;

        public PlayerPreferences(PlayerHand dominantHand, /* float shoulderHeight, */SwitchColorInputType switchColorInputType, float flashIntensity)
        {
            this.dominantHand = dominantHand;
            //this.shoulderHeight = shoulderHeight;
            this.switchColorInputType = switchColorInputType;
            this.flashIntensity = flashIntensity;
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
    [Range(1f,2f)]
    public float flashIntensityDefaultValue = 2;

    #endregion

    public PlayerHand PlayerDominantHand 
    {
        get => playerPreferences.dominantHand;
        set
        {
            if(IsDominantHandValid(value))
            {
                playerPreferences.dominantHand = value;
                SavePlayerSettings();
            }
        }
    }
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
    public SwitchColorInputType SwitchColorInputType
    {
        get => playerPreferences.switchColorInputType;
        set 
        {
            if(IsSwitchColorInputTypeValid(value))
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
            if (IsFlashIntensityValid(value))
            {
                playerPreferences.flashIntensity = value;
                SavePlayerSettings();
            }
                
        }
    }

    private PlayerPreferences playerPreferences;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPlayerSettings();
    }

    public void LoadPlayerSettings()
    {
        if(System.IO.File.Exists(Application.persistentDataPath + "/PlayerSettings.json"))
        {
            string preferencesToLoad = System.IO.File.ReadAllText(Application.persistentDataPath + "/PlayerSettings.json");
            playerPreferences = JsonUtility.FromJson<PlayerPreferences>(preferencesToLoad);
            if(!CheckPreferencesIntegrity(playerPreferences))
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
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerSettings.json", preferencesToSave);
    }

    private PlayerPreferences CreateNewPlayerPreferences()
    {
        PlayerPreferences newDefaultPlayerSettings = new PlayerPreferences(dominantHandDefaultValue,/* shoulderHeightDefaultValue,*/ switchColorInputTypeDefaultValue, flashIntensityDefaultValue);

        return newDefaultPlayerSettings;
    }

    private bool CheckPreferencesIntegrity(PlayerPreferences preferrences)
    {
        bool preferencesWasValid = true;

        if(!IsDominantHandValid(preferrences.dominantHand))
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

        if(!IsFlashIntensityValid(preferrences.flashIntensity))
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
