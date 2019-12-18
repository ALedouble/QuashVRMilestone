using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputable
{
    void OnRightTriggerPress();

    void OnRightTriggerRelease();

    void OnLeftTriggerPress();

    void OnLeftTriggerRelease();
}
