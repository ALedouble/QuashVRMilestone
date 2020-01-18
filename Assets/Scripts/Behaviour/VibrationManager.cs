using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VibrationManager : MonoBehaviour
{
    public VRTK_ControllerReference controllerRef;
    public AudioClip clipVib;

    public static VibrationManager instance;


    private void Awake()
    {
        instance = this;
    }

    public void StartVibration()
    {
        controllerRef = VRTK_ControllerReference.GetControllerReference(1);
        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, clipVib);
    }
}
