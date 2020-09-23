using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetSelector
{
    void SwitchTarget();
    QPlayer CurrentTargetPlayer { get; }
    Vector3 GetTargetPlayerPosition();
    Vector3 GetNewTargetPosition();
    void SetCurrentTargetPlayer(QPlayer newTarget);
    QPlayer GetPreviousTargetPlayer();
}
