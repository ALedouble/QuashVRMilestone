using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLinker : MonoBehaviour
{
    public GameObject leftControllerModel, rightControllerModel;

    public void OnRightTriggerPress()
    {
        PlayerInputManager.instance?.OnRightTriggerPress();
    }

    public void OnRightTriggerRelease()
    {
        PlayerInputManager.instance?.OnRightTriggerRelease();
    }

    public void OnLeftTriggerPress()
    {
        PlayerInputManager.instance?.OnLeftTriggerPress();
    }

    public void OnLeftTriggerRelease()
    {
        PlayerInputManager.instance?.OnLeftTriggerRelease();
    }

    public void OnLeftStartButtonPress()
    {
        Debug.Log("LeftStartButtonPressed");
        PlayerInputManager.instance?.OnStartButtonPress();
    }

    public void OnRightStartButtonPress()
    {
        Debug.Log("RightStartButtonPressed");
        PlayerInputManager.instance?.OnStartButtonPress();
    }

    public void ControllerModelSetActive(bool value)
    {
        leftControllerModel.SetActive(value);
        rightControllerModel.SetActive(value);
    }
}
