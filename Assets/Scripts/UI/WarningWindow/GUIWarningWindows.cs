using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIWarningWindows : MonoBehaviour
{
    [Header("Text values settings")]
    public string windowTitle = "!!! Warning !!!";
    public string windowText = "Warning Text";
    public string buttonText = "Ok";

    [Header("References")]
    public TextMeshProUGUI textMeshTitle;
    public TextMeshProUGUI textMeshText;
    public TextMeshProUGUI textMeshButton;

    //delegate
    public delegate void WarningWindowDelegate();
    public event WarningWindowDelegate OnButtonPressed;

    protected virtual void OnEnable()
    {
        // SetWarningText();
    }

    public virtual void SetWarningText()
    {
        textMeshTitle.text = windowTitle;
        textMeshText.text = windowText;
        textMeshButton.text = buttonText;
    }

    public virtual void OverrideText(string p_title, string p_text, string p_button = "Ok")
    {
        windowTitle = p_title;
        windowText = p_text;
        buttonText = p_button;
    }

    public virtual void OnValidateButtonClicked()
    {
        OnButtonPressed?.Invoke();
        Destroy(gameObject);
    }
}
