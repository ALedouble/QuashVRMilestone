using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TargetSelector
{
    void SwitchTarget();
    Vector3 GetTargetPosition();
    void SetCurrentTarget(QPlayer newTarget);
    QPlayer GetCurrentTarget();
}
