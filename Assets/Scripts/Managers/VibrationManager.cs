using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VibrationManager : MonoBehaviour
{
    public VRTK_ControllerReference controllerRef;
    public Vibration[] vibrations;

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

    public void VibrateOn(float strength, float duration, float pulseInterval)
    {
        controllerRef = VRTK_ControllerReference.GetControllerReference((uint)QPlayerManager.instance.GetMainHand());
        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, strength, duration, pulseInterval);
    }

    public void VibrationOff(VRTK_ControllerReference controller)
    {
        controllerRef = controller;
        VRTK_ControllerHaptics.CancelHapticPulse(controllerRef);
    }

    public void VibrateOn(string vibrationName)
    {
        bool vibrationFound = false;
        int index = 0;

        for (int i = 0; i < vibrations.Length; i++)
        {
            if (vibrationName == vibrations[i].vibrationName)
            {
                index = i;
                vibrationFound = true;
                break;
            }
        }

        if (!vibrationFound)
        {
            Debug.LogError("Vibration Not Found");
            return;
        }

        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
        else
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);

        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, vibrations[index].audioClip);
    }

}

[System.Serializable]
public class Vibration
{
    public AudioClip audioClip;
    public string vibrationName = "vibration_name";
}
