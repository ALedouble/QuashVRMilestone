using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    public static OptionMenu Instance;

    public GameObject colorSwitchInputHoldButton;
    public GameObject colorSwitchInputClickButton;

    public GameObject flashIntensitySlider;

    private ColorSwitchInputButton holdButtonScript;
    private ColorSwitchInputButton clickButtonScript;

    private void Awake()
    {
        Instance = this;

        holdButtonScript = colorSwitchInputHoldButton.GetComponent<ColorSwitchInputButton>();
        clickButtonScript = colorSwitchInputClickButton.GetComponent<ColorSwitchInputButton>();
    }

    private void OnEnable()
    {
        UpdateOptionMenuDisplay();
    }

    private void UpdateOptionMenuDisplay()
    {
        UpdateColorSwitchInputButtons();
        UpdateFlashIntensitySlider();
    }

    public void UpdateColorSwitchInputButtons()
    {
        holdButtonScript.UpdateBackground();
        clickButtonScript.UpdateBackground();
    }

    public void UpdateFlashIntensitySlider()
    {

    }
}
