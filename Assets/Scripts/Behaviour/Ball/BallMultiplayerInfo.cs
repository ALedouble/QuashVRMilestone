using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMultiplayerInfo : MonoBehaviour
{
    public static BallMultiplayerInfo Instance;

    public bool IsBallOwner { get; private set; }

    public void BecomeBallOwner()
    {
        IsBallOwner = true;
    }

    public void LoseBallOwnership()
    {
        IsBallOwner = false;
    }
}
