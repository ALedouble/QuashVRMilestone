using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VibrationManager : MonoBehaviour
{
    public VRTK_ControllerReference controllerRef;

    public static VibrationManager instance;



    private void Awake()
    {
        instance = this;
    }

    public void VibrateOn(AudioClip clipVib)
    {
        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
        else
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);

        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, clipVib);
    }

    public void VibrateOn(float strength)
    {
        controllerRef = VRTK_ControllerReference.GetControllerReference((uint)QPlayerManager.instance.GetMainHand());
        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, strength);
    }
}
