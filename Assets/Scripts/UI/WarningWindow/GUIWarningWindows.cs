using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIWarningWindows : MonoBehaviour
{
    [Header("Text values settings")]
     public string windowTitle;
     public string windowText;
     public string buttonText;

    [Header("References")]
    [SerializeField]public TextMeshProUGUI textMeshTitle;
    [SerializeField] public TextMeshProUGUI textMeshText;
    [SerializeField] public TextMeshProUGUI textMeshButton;

    //delegate
    public delegate void WarningWindowDelegate();
    public event WarningWindowDelegate OnButtonPressed;

    protected virtual void OnEnable()
    {
       // SetWarningText();
    }

    public void SetWarningText()
    {
        textMeshTitle.text = windowTitle;
        textMeshText.text = windowText;
        textMeshButton.text = buttonText;
    }

    public virtual void OnValidateButtonClicked()
    {
        OnButtonPressed?.Invoke();
        Destroy(gameObject);
    }
}
