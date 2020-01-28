﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class BrickBehaviours : MonoBehaviourPunCallbacks/*, IPunObservable*/
{

    public static int brickCount = 0;
    private int brickID;
    public int BrickID { get => brickID; }
    

    [Header("Waypoint")]
    public bool isMoving;
    [Tooltip("Enter waypoints positions here")]
    public List<Vector3> waypoints;

    private int waypointIndex;
    private Vector3 refVector;

    [Header("Waiting Parameters")]
    public bool hasToWait;
    public float waitFor;

    [Header("Move Modifiers")]
    [Range(0, 1)]
    [Tooltip("Damping strength")]
    public float smoothTime;
    [Tooltip("Speed of the brick")]
    [Range(0.1f, 10)]
    public float speed;
    private bool isWaiting;                                                       // Pour l'instant ça sert à rien...?                                   

    [Header("Pattern")]
    [Tooltip("Is the brick going backward when reaching its last waypoint ?")]
    public bool turningBack;
    private bool onItsWayBack;

    [Header("Saved Value")]
    public int savedInIndex; //index de la Brick dans la couche


    BrickInfo brickInfo;
    bool hasBeenHit;

    private void Awake()
    {
        

        if (isMoving)
        {
            if (waypoints.Count != 0)
            {
                this.transform.position = waypoints[waypointIndex];

                waypointIndex++;
            }
        }

    }
    
    void Start()
    {
        SetupBallID();

        isWaiting = false;
    }


    private void SetupBallID()
    {
        brickID = brickCount++;
        brickInfo = GetComponent<BrickInfo>();
        BrickManager.Instance.AddBrick(gameObject);
    }

    private void Update()
    {
        if (isMoving && GameManager.Instance.IsReady)
        {
            Moving();
        }
    }

    private void Moving()
    {
        if(!isWaiting)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, waypoints[waypointIndex],
            speed * 0.009f); 

            if (this.transform.position == waypoints[waypointIndex])
            {
                Debug.Log("Reached");
                if (hasToWait)
                {
                    isWaiting = true;
                    StartCoroutine(WaitUntil(waitFor));
                }
                else
                {
                    NextWaypoint();
                }
            }
        }
        
    }

    /// <summary>
    /// Définition du prochain waypoint
    /// </summary>
    private void NextWaypoint()
    {
        if (turningBack)
        {
            if (onItsWayBack)
            {
                if (waypointIndex > 0)
                {
                    waypointIndex--;
                }
                else
                {
                    onItsWayBack = false;
                    waypointIndex++;
                }
            }
            else
            {
                if (waypointIndex < waypoints.Count - 1)
                {
                    waypointIndex++;
                }
                else
                {
                    onItsWayBack = true;
                    waypointIndex--;
                }
            }
        }
        else
        {
            if (waypointIndex < waypoints.Count - 1)
            {
                waypointIndex++;
            }
            else
            {
                waypointIndex = 0;
            }
        }
    }

    IEnumerator WaitUntil(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        isWaiting = false;

        NextWaypoint();
    }


    

    [PunRPC]
    public void HitBrick(int p_dmgPoints = 1)
    {
        Debug.Log("Hit" + hasBeenHit);
        if (!hasBeenHit)
        {
            hasBeenHit = true;

            brickInfo.armorValue--;

            Debug.Log("Armor" + brickInfo.armorValue);
            if (brickInfo.armorValue <= 0)
            {
                Debug.Log("Go to destroy");
                hasBeenHit = false;
                AudioManager.instance.PlaySound("SFX_Brick_Explosion", Vector3.zero);
                DestroyBrick();
            }
            //else
            //{
            //    StartCoroutine(HitRecoverDelay(1));
            //}

        }
        //other case scenario
    }

    //IEnumerator HitRecoverDelay(float recoverTime)
    //{
    //    yield return new WaitForSeconds(recoverTime);

    //    hasBeenHit = false;
    //}

    /// <summary>
    /// Détruit la brique, augmente le score, renvoie les feedbacks et spawn les bonus/malus
    public void DestroyBrick()
    {
        Debug.Log("Destroy");
        DespawnBrick();

        SendBreakFeedbacks();

        ScorePoints();

        DropBonusMalus();

        //AudioManager.instance.PlayHitSound(soundName, touchedBrick.Transform.position, touchedBrick.Transform.rotation, hitIntensity);
    }

    private void DespawnBrick()
    {
        Debug.Log("Despawn");
        gameObject.SetActive(false);
        transform.parent = null;
        BrickManager.Instance.UpdateBrickLevel(brickInfo.wallID);
    }

    private void SendBreakFeedbacks()
    {
        /// FX
        switch (brickInfo.colorID)
        {
            case 0:
                PoolManager.instance.SpawnFromPool("Brick_Destroyed_White", transform.position + new Vector3(0, 0, -0.5f), Quaternion.LookRotation(transform.forward, Vector3.up));
                break;

            case 1:
                PoolManager.instance.SpawnFromPool("Brick_Destroyed_Blue", transform.position + new Vector3(0, 0, -0.5f), Quaternion.LookRotation(transform.forward, Vector3.up));
                break;

            case 2:
                PoolManager.instance.SpawnFromPool("Brick_Destroyed_Green", transform.position + new Vector3(0, 0, -0.5f), Quaternion.LookRotation(transform.forward, Vector3.up));
                break;
        }
        //PoolManager.instance.SpawnFromPool("CubeDeathFX", transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));

        ///Skake
        LevelManager.instance.ShakeLayer(brickInfo.wallID);         //WTF?
    }

    

    private void ScorePoints()
    {
        /// Score

        ScoreManager.Instance.BuildScoreText(brickInfo.scoreValue, brickInfo.colorID, transform.position, transform.rotation);

        
        ScoreManager.Instance.SetScore(brickInfo.scoreValue, (int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
        ScoreManager.Instance.SetCombo((int)BallManager.instance.GetLastPlayerWhoHitTheBall()); //BallID
        

        ScoreManager.Instance.resetCombo = false;
    }

    private void DropBonusMalus()
    {
        //Bonus & malus case
        if (brickInfo.isBonus) BonusManager.instance.SpawnRandomObject(transform);
        if (brickInfo.isMalus) MalusManager.instance.SpawnRandomObject(transform);
    }

    public static void ResetBrickCount()
    {
        brickCount = 0;
    }

    #region IPunObservable implementation
    //void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(transform.position);
    //        stream.SendNext(transform.rotation);

    //    }
    //    else
    //    {
    //        transform.position = (Vector3)stream.ReceiveNext();
    //        transform.rotation = (Quaternion)stream.ReceiveNext();
    //    }
    //}
    #endregion
}
