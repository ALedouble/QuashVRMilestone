using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLinker : MonoBehaviour
{
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

    public void OnLeftPauseButtonPress()
    {
        PlayerInputManager.instance?.OnPauseButtonPress();
    }

    public void OnRightPauseButtonPress()
    {
        PlayerInputManager.instance?.OnPauseButtonPress();
    }
}
