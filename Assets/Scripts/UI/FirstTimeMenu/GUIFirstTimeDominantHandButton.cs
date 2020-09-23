using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIFirstTimeDominantHandButton : MonoBehaviour
{
    public PlayerHand buttonHand;

    public GameObject selectedBackground;
    public GameObject notSelectedBackground;

    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            UpdateBackground(value);
        }
    }

    public void OnButtonPress()
    {
        GUIFirstTimeMenu.Instance.SelectDominantHand(buttonHand);
    }

    private void UpdateBackground(bool isSelected)
    {
        selectedBackground.SetActive(isSelected);
        notSelectedBackground.SetActive(!isSelected);
    }
}
