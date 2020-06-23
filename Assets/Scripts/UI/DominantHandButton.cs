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
            PlayerSettings.Instance.PlayerDominantHand = buttonHand;
    }

    public void SetBackground(bool isActive)
    {
        activeBackground.SetActive(isActive);
        inactiveBackground.SetActive(!isActive);
    }
}
