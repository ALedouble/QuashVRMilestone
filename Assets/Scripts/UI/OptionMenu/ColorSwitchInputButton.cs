using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwitchInputButton : MonoBehaviour
{
    public SwitchColorInputType buttonColorSwitchInputType;

    public GameObject activeBackground;
    public GameObject inactiveBackground;

    public void OnColorSwitchInputButtonClick()
    {
        if (PlayerSettings.Instance.SwitchColorInputType != buttonColorSwitchInputType)
        {
            PlayerSettings.Instance.SwitchColorInputType = buttonColorSwitchInputType;
            OptionMenu.Instance.UpdateColorSwitchInputButtons();
        }
    }

    public void UpdateBackground()
    {
        activeBackground.SetActive(PlayerSettings.Instance.SwitchColorInputType == buttonColorSwitchInputType);
        inactiveBackground.SetActive(PlayerSettings.Instance.SwitchColorInputType != buttonColorSwitchInputType);
    }
}
