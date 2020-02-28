using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIWarningWindows : MonoBehaviour
{
    [Header("Text values settings")]
    [SerializeField] string windowTitle = "!!! Warning !!!";
    [SerializeField] string windowText = "Warning Text";
    [SerializeField] string buttonText = "Ok";

    [Header("References")]
    [SerializeField] TextMeshProUGUI textMeshTitle;
    [SerializeField] TextMeshProUGUI textMeshText;
    [SerializeField] TextMeshProUGUI textMeshButton;

    //delegate
    public delegate void WarningWindowDelegate();
    public event WarningWindowDelegate OnButtonPressed;

    protected virtual void OnEnable()
    {
        SetWarningText();
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
