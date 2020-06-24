using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSettings : MonoBehaviour
{
    [System.Serializable]
    private class PlayerPreferences
    {
        public int dominantHand;
        public float shoulderHeight;
    }


    public static PlayerSettings Instance;
    public PlayerHand PlayerDominantHand 
    {
        get => (PlayerHand)playerPreferences.dominantHand;
        set
        {
            if((int)value > 0 && (int)value < 3)
            {
                playerPreferences.dominantHand = (int)value;
                SavePlayerSettings();

                DominantHandMainMenu.Instance?.UpdateDominantHandButton();
            }
        }
    }
    public float PlayerShoulderHeight 
    {
        get => playerPreferences.dominantHand;
        set
        {
            if(playerPreferences.shoulderHeight < -2f || playerPreferences.shoulderHeight > 4f)
            {
                playerPreferences.shoulderHeight = value;
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
            CheckPreferenceIntegrity();
        }
        else
        {
            //CreateNewPlayerSettings();
            CreateDefaultPlayerSettings();
        }
    }

    public void SavePlayerSettings()
    {
        string preferencesToSave = JsonUtility.ToJson(playerPreferences);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerSettings.json", preferencesToSave);
    }

    public void CreateNewPlayerSettings()
    {

    }

    private void CheckPreferenceIntegrity()
    {
        if (!AreFieldValid())
            //CreateNewPlayerSettings();
            CreateDefaultPlayerSettings();
    }

    private bool AreFieldValid()
    {
        bool isValid = true;

        if (playerPreferences.dominantHand < 0 || playerPreferences.dominantHand > 3)
            isValid = false;

        if (playerPreferences.shoulderHeight < -2f || playerPreferences.shoulderHeight > 4f)
            isValid = false;

        return isValid;
    }

    private void CreateDefaultPlayerSettings()
    {
        playerPreferences = new PlayerPreferences();
        playerPreferences.dominantHand = 2;
        playerPreferences.shoulderHeight = 1f;

        SavePlayerSettings();
    }
}
