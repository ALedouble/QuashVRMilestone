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

    public delegate void OnCollisionDelegate(Collision collision);
    public delegate void OnCollisionExitDelegate();
    public delegate void OnBallEventDelegate();

    public event OnCollisionDelegate OnCollisionWithBrick;
    public event OnCollisionDelegate OnCollisionWithFrontWall;
    public event OnCollisionDelegate OnCollisionWithWall;
    public event OnCollisionDelegate OnCollisionWithFloor;
    public event OnCollisionDelegate OnCollisionWithRacket;
    public event OnCollisionDelegate OnCollisionWithBackWall;

    public event OnCollisionExitDelegate OnCollisionExitWithFloor;

    public event OnBallEventDelegate OnBallSpawn;
    public event OnBallEventDelegate OnBallDespawn;
    public event OnBallEventDelegate OnLoseBall;

    public event OnBallEventDelegate OnBallColorSwitch;

    public void OnBallCollision(string tag, Collision collision)
    {
        switch (tag)
        {
            case "Racket":
                if(OnCollisionWithRacket != null)
                    OnCollisionWithRacket(collision);
                break;
            case "Wall":
                if (OnCollisionWithWall != null)
                    OnCollisionWithWall(collision);
                break;
            case "Floor":
                if (OnCollisionWithFloor != null)
                    OnCollisionWithFloor(collision);
                break;
            case "FrontWall":
                if (OnCollisionWithFrontWall != null)
                    OnCollisionWithFrontWall(collision);
                break;
            case "Brick":
                if (OnCollisionWithBrick != null)
                    OnCollisionWithBrick(collision);
                break;
            case "BackWall":
                if (OnCollisionWithBackWall != null)
                    OnCollisionWithBackWall(collision);
                break;
            default:
                break;
        }
    }

    public void OnBallCollisionExit(string tag)
    {
        switch(tag)
        {
            case "Floor":
                if (OnCollisionExitWithFloor != null)
                    OnCollisionExitWithFloor();
                break;
            default:
                break;
        }
    }

    public void SendBallColorSwitchEvent()
    {
        //Debug.Log("BallColorSwitchEvent");
        OnBallColorSwitch?.Invoke();
    }
}


