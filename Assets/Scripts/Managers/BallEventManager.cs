using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEventManager : MonoBehaviour
{
    public static BallEventManager instance;

    public event Action OnCollisionWithBrick = delegate { };
    public event Action OnCollisionWithFrontWall = delegate { };
    public event Action OnCollisionWithWall = delegate { };
    public event Action OnCollisionWithRacket = delegate { };

    private void Awake()
    {
        instance = this;
    }

    public void OnBallCollision(BallCollisionInfo ballCollisionInfo)
    {
        switch (ballCollisionInfo.OtherTag)
        {
            case "Racket":
                OnCollisionWithRacket();
                break;
            case "Wall":
                OnCollisionWithWall();
                break;
            case "FrontWall":
                OnCollisionWithFrontWall();
                break;
            case "Brick":
                OnCollisionWithBrick();
                break;
            default:
                break;
        }
    }
}


