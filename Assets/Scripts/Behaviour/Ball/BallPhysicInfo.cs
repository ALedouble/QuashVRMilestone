using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysicInfo : MonoBehaviour
{
    private bool isOnFrontWallCollisionFrame;
    public bool IsOnFrontWallCollisionFrame
    {
        get => isOnFrontWallCollisionFrame;
        set
        {
            if (value == true)
                StartCoroutine(ResetFrontWallCollisionBoolValue());

            isOnFrontWallCollisionFrame = value;
        }
    }

    #region FrontWallCollision

    private IEnumerator ResetFrontWallCollisionBoolValue()
    {
        do
        {
            yield return new WaitForFixedUpdate();
            isOnFrontWallCollisionFrame = false;
        }
        while (BallManager.instance.IsBallPaused);
    }

    #endregion
}
