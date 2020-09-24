using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public static OptionMenu Instance;

    public GameObject leftHandButton;
    public GameObject rightHandButton;

    public GameObject colorSwitchInputHoldButton;
    public GameObject colorSwitchInputClickButton;

    public Slider flashIntensitySlider;

    private OptionMenuDominantHandButton leftHandButtonScript;
    private OptionMenuDominantHandButton rightHandButtonScript;

    private ColorSwitchInputButton holdButtonScript;
    private ColorSwitchInputButton clickButtonScript;

    private void Awake()
    {
        Instance = this;

        leftHandButtonScript = leftHandButton.GetComponent<OptionMenuDominantHandButton>();
        rightHandButtonScript = rightHandButton.GetComponent<OptionMenuDominantHandButton>();

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
        UpdateDominantHandButtons();
        UpdateColorSwitchInputButtons();
        UpdateFlashIntensitySlider();
    }

    public void UpdateDominantHandButtons()
    {
        leftHandButtonScript.UpdateBackground();
        rightHandButtonScript.UpdateBackground();
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
