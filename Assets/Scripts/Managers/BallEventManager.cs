using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallEventManager : MonoBehaviour
{
    #region Singleton
    public static BallEventManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public delegate void OnCollisionDelegate(BallCollisionInfo ballCollisionInfo);

    public event OnCollisionDelegate OnCollisionWithBrick;
    public event OnCollisionDelegate OnCollisionWithFrontWall;
    public event OnCollisionDelegate OnCollisionWithWall;
    public event OnCollisionDelegate OnCollisionWithRacket;

    [PunRPC]
    public void OnBallCollision(BallCollisionInfo ballCollisionInfo)
    {
        switch (tag)
        {
            case "Racket":
                OnCollisionWithRacket(ballCollisionInfo);
                break;
            case "Wall":
                OnCollisionWithWall(ballCollisionInfo);
                break;
            case "FrontWall":
                OnCollisionWithFrontWall(ballCollisionInfo);
                break;
            case "Brick":
                OnCollisionWithBrick(ballCollisionInfo);
                break;
            default:
                break;
        }
    }
}


