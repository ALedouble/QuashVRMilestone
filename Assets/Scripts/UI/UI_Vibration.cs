using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UI_Vibration : MonoBehaviour
{
    public VRTK_UIPointer uip;
    private VRTK_ControllerReference controllerRef;

    public void VibrateOn(float strength)
    {
        controllerRef = VRTK_ControllerReference.GetControllerReference(uip.controllerEvents.gameObject);

        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, strength);
    }

    public void PRINTING(string text)
    {
        Debug.Log(text);
    }
}
