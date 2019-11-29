using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBrick
{
    bool IsBonus { get; }
    bool IsMalus { get; }

    void HitBrick(int p_dmgPoints = 1);

    BrickInfo GetBrickInfo();
}
