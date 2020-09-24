using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuDominantHandButton : MonoBehaviour
{
    public PlayerHand buttonHand;

    public GameObject activeBackground;
    public GameObject inactiveBackground;

    public void OnDominantHandButtonClick()
    {
        if (PlayerSettings.Instance.PlayerDominantHand != buttonHand)
        {
            PlayerSettings.Instance.PlayerDominantHand = buttonHand;
            OptionMenu.Instance.UpdateDominantHandButtons();
        }
    }

    public void UpdateBackground()
    {
        activeBackground.SetActive(PlayerSettings.Instance.PlayerDominantHand == buttonHand);
        inactiveBackground.SetActive(PlayerSettings.Instance.PlayerDominantHand != buttonHand);
    }
}
