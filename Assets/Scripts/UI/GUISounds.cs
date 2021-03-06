﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISounds : MonoBehaviour
{
    public void PlayBackButtonClick()
    {
        AudioManager.instance.PlaySound("UI_Back_Button_Click", Vector3.zero);
    }

    public void PlayButtonClick()
    {
        AudioManager.instance.PlaySound("UI_Button_Click", Vector3.zero);
    }

    public void PlayValidateEntry()
    {
        AudioManager.instance.PlaySound("UI_Validate_Entry", Vector3.zero);
    }

    public void PlayWindowAppear()
    {
        AudioManager.instance.PlaySound("UI_Window_Appear", Vector3.zero);
    }

    public void PlayMistakeSound()
    {
        AudioManager.instance.PlaySound("Mistake", Vector3.zero);
    }


    public void PlayOnHover()
    {
        AudioManager.instance.PlaySound("UI_On_Hover", Vector3.zero);
        VibrationManager.instance.VibrateOn("Vibration_UI_OnHover");
    }
}
