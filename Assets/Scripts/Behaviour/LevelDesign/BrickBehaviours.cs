﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class BrickBehaviours : MonoBehaviourPunCallbacks, IBrick, IPunObservable
{
    [Header("Score Modifier")]
    public int scoreValue;

    [Header("Armor")]
    [Tooltip("How many hit BEFORE the next one kill it")]
    public int armorPoints = 1;
    bool hasBeenHit;

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
    private bool isWaiting;

    [Header("Pattern")]
    [Tooltip("Is the brick going backward when reaching its last waypoint ?")]
    public bool turningBack;
    private bool onItsWayBack;

    [Header("Bonus/Malus")]
    public bool isBonus;
    public bool isMalus;

    [Header("Color ID")]
    public int colorID; //Couleur du preset ID

    [Header("Wall ID")]
    public int wallID; //Mur du joueur ID

    [Header("Saved Value")]
    public int savedInIndex; //index de la Brick dans la couche



    public bool IsBonus { get => isBonus; }
    public bool IsMalus { get => isMalus; }
    public int ColorID { get => colorID; private set => colorID = value; }





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

    private void Update()
    {
        if (isMoving)
        {
            Moving();
        }
    }

    private void Moving()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, waypoints[waypointIndex],
            speed * 0.009f);

        if (this.transform.position == waypoints[waypointIndex])
        {
            //Debug.Log("Reached");
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


    public void HitBrick(int p_dmgPoints = 1)
    {
        if (!hasBeenHit)
        {
            //Debug.Log("damage of " + p_dmgPoints);
            hasBeenHit = true;

            armorPoints--;


            if (armorPoints <= 0)
            {
                if (TryGetComponent<IBrick>(out IBrick brick))
                {
                    hasBeenHit = false;
                    AudioManager.instance.PlaySound("SFX_Brick_Explosion", Vector3.zero);
                    BrickManager.Instance.DeadBrick(brick.GetBrickInfo());
                }
            }
            else
            {
                StartCoroutine(HitRecoverDelay(1));
            }

        }

        //other case scenario
    }

    IEnumerator HitRecoverDelay(float recoverTime)
    {
        yield return new WaitForSeconds(recoverTime);

        hasBeenHit = false;
    }











    public BrickInfo GetBrickInfo()
    {
        return new BrickInfo(scoreValue, armorPoints,ColorID, wallID, IsBonus, isMalus, transform);
    }

    #region IPunObservable implementation
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
    #endregion
}

public class BrickInfo
{
    int scoreValue;
    int armorValue;
    int colorID;
    int wallID;
    bool isBonus;
    bool isMalus;
    Transform transform;

    public BrickInfo(int p_score,int p_armor,int p_colorID, int p_wallID, bool p_isBonus, bool p_isMalus, Transform p_transform)
    {
        ScoreValue = p_score;
        ArmorValue = p_armor;
        ColorID = p_colorID;
        WallID = p_wallID; 
        IsBonus = p_isBonus;
        IsMalus = p_isMalus;
        Transform = p_transform;
    }

    public int ScoreValue { get => scoreValue; private set => scoreValue = value; }
    public int ArmorValue { get => armorValue; private set => armorValue = value; }
    public int ColorID { get => colorID; private set => colorID = value; }
    public int WallID { get => wallID; private set => wallID = value; }
    public bool IsBonus { get => isBonus; private set => isBonus = value; }
    public bool IsMalus { get => isMalus; private set => isMalus = value; }
    public Transform Transform { get => transform; private set => transform = value; }


    
}
