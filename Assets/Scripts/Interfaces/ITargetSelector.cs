using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetSelector
{
    void SwitchTarget();
    Vector3 GetTargetPlayerPosition();
    Vector3 GetNewTargetPosition();
    void SetCurrentTarget(QPlayer newTarget);
    QPlayer GetCurrentTarget();
}
