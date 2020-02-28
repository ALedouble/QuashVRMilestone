using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontWallBehaviour : MonoBehaviour
{
    public void Initialize()
    {
        // BallManager truc truc
    }

    private void EnterProtectionState()
    {
        transform.localScale = new Vector3(1f, 1f, 7.5f);
    }

    private void ExitProtectionState()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
