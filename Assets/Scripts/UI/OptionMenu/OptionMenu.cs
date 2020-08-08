using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public static OptionMenu Instance;

    public GameObject colorSwitchInputHoldButton;
    public GameObject colorSwitchInputClickButton;

    public Slider flashIntensitySlider;

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
        flashIntensitySlider.onValueChanged.AddListener(delegate { ApplyFlashIntensityChange(); });
    }

    private void OnDisable()
    {
        flashIntensitySlider.onValueChanged.RemoveAllListeners();
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
        flashIntensitySlider.value = PlayerSettings.Instance.FlashIntensity;
    }

    private void ApplyFlashIntensityChange()
    {
        PlayerSettings.Instance.FlashIntensity = flashIntensitySlider.value;
        //Faire un flash test
    }
}
