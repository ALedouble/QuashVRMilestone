using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VibrationManager : MonoBehaviour
{
    public VRTK_ControllerReference controllerRef;
    public Vibration[] vibrations;

    private bool vibrationOnRepeat;

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

    public void VibrateOnDuration(AudioClip clipVib, float duration)
    {
        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
        else
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);

        VRTK_ControllerHaptics.TriggerHapticPulseOnDuration(controllerRef, clipVib, duration);
    }

    public void VibrateOnRepeat(AudioClip clipVib)
    {
        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
        else
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);

        VRTK_ControllerHaptics.TriggerHapticPulseOnRepeat(controllerRef, clipVib);
    }


    public void VibrateOn(float strength)
    {

        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
        else
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);

        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, strength);
    }

    /// <summary>
    /// Lancer plusieurs vibrations d'une certain puissance pendant une certaine durée à intervalle régulier
    /// </summary>
    /// <param name="strength">Puissance de la vibration</param>
    /// <param name="duration">Durée de la vibration</param>
    /// <param name="pulseInterval">Temps entre chaque vibration</param>
    public void VibrateOnDuration(float strength, float duration, float pulseInterval)
    {
        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
        else
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);

        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, strength, duration, pulseInterval);
    }

    public void VibrateOnRepeat(float strength, float pulseInterval)
    {
        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
        else
            controllerRef = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);

        VRTK_ControllerHaptics.TriggerHapticPulse(controllerRef, strength, pulseInterval);
    }


    /// <summary>
    /// Lancer une vibration selon un string faisant référence à une array
    /// </summary>
    /// <param name="vibrationName"></param>
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

    /// <summary>
    /// Lancer une vibration selon un string faisant référence à une array
    /// </summary>
    /// <param name="vibrationName"></param>
    public void VibrateOnDuration(string vibrationName, float duration)
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

        VRTK_ControllerHaptics.TriggerHapticPulseOnDuration(controllerRef, vibrations[index].audioClip, duration);
    }

    /// <summary>
    /// Lancer une vibration selon un string faisant référence à une array
    /// </summary>
    /// <param name="vibrationName"></param>
    public void VibrateOnRepeat(string vibrationName)
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

        VRTK_ControllerHaptics.TriggerHapticPulseOnRepeat(controllerRef, vibrations[index].audioClip);
    }



    /// <summary>
    /// Stop une vibration
    /// </summary>
    /// <param name="controller">Controller cencerné</param>
    public void VibrationOff(VRTK_ControllerReference controller)
    {
        controllerRef = controller;
        VRTK_ControllerHaptics.CancelHapticPulse(controllerRef);


        //if(vibrationOnRepeat)
            
    }

    /// <summary>
    /// Ends a controller's vibration
    /// </summary>
    /// <param name="controller">Which controller will you stop ?</param>
    /// <param name="time">Seconds of vibrations left before it stops</param>
    /// <returns></returns>
    IEnumerator StopVibrationIn(VRTK_ControllerReference controller, float time)
    {
        yield return new WaitForSeconds(time);

        VibrationOff(controller);
    }



}

[System.Serializable]
public class Vibration
{
    public AudioClip audioClip;
    public string vibrationName = "vibration_name";
}
