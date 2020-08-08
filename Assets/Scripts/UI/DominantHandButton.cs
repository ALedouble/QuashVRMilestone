using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominantHandButton : MonoBehaviour
{
    public PlayerHand buttonHand;

    public GameObject activeBackground;
    public GameObject inactiveBackground;

    public void OnDominantHandButtonClick()
    {
        if(PlayerSettings.Instance.PlayerDominantHand != buttonHand)
        {
            PlayerSettings.Instance.PlayerDominantHand = buttonHand;
            DominantHandMainMenu.Instance.UpdateDominantHandButton();
        } 
    }

    public void UpdateBackground()
    {
        activeBackground.SetActive(PlayerSettings.Instance.PlayerDominantHand == buttonHand);
        inactiveBackground.SetActive(PlayerSettings.Instance.PlayerDominantHand != buttonHand);
    }
}
