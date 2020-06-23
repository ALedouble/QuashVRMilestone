using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings Instance;
    public PlayerHand PlayerDominantHand { get; set; }
    //PlayerSize

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Default Need proper settings
        PlayerDominantHand = PlayerHand.RIGHT;
    }

    public void LoadPlayerSettings()
    {
        
    }

    public void SavePlayerSettings()
    {

    }
}
