using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEventManager : MonoBehaviour
{
    #region Singleton
    public static BallEventManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public delegate void OnCollisionDelegate();

    public event OnCollisionDelegate OnCollisionWithBrick;
    public event OnCollisionDelegate OnCollisionWithFrontWall;
    public event OnCollisionDelegate OnCollisionWithWall;
    public event OnCollisionDelegate OnCollisionWithRacket;


    //public void OnBallCollision(BallCollisionInfo ballCollisionInfo)
    public void OnBallCollision(string tag)
    {
        switch (tag)
        {
            case "Racket":
                if(OnCollisionWithRacket != null)
                    OnCollisionWithRacket();
                break;
            case "Wall":
                if (OnCollisionWithWall != null)
                    OnCollisionWithWall();
                break;
            case "FrontWall":
                if (OnCollisionWithFrontWall != null)
                    OnCollisionWithFrontWall();
                break;
            case "Brick":
                if (OnCollisionWithBrick != null)
                    OnCollisionWithBrick();
                break;
            default:
                break;
        }
    }
}


