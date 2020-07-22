using System.Collections;
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
    public bool isAMovingBrick;
    [Tooltip("Enter waypoints positions here")]
    public List<Vector3> waypoints;

    private int waypointIndex;

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
    private float distanceTravelled;
    Vector3 currentPosition;
    Vector3 nextPosition;

    [Header("Saved Value")]
    public int savedInIndex; //index de la Brick dans la couche


    BrickInfo brickInfo;
    bool hasBeenHit;

    private void Awake()
    {
        if (isAMovingBrick)
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
        if (isAMovingBrick && GameManager.Instance.IsBrickFreeToMove)
        {
            Moving();
        }
    }

    private void Moving()
    {
        /// Passage du SmoothDamp au Towards
        float newSpeed = speed * 0.009f;

        if (!isWaiting)
        {
            nextPosition = Vector3.MoveTowards(this.transform.position, waypoints[waypointIndex], newSpeed);

            if (nextPosition == waypoints[waypointIndex])
            {
                distanceTravelled = Vector3.Distance(this.transform.position, waypoints[waypointIndex]);
                if (hasToWait)
                {
                    isWaiting = true;
                    StartCoroutine(WaitUntil(waitFor));
                }
                else
                {
                    NextWaypoint();
                }

                if (distanceTravelled < newSpeed)
                {
                    nextPosition = Vector3.MoveTowards(nextPosition, waypoints[waypointIndex], newSpeed - distanceTravelled);
                }
            }

            this.transform.position = nextPosition;

            #region Old Moving System
            ///////OLD Version/////////
            /*
            this.transform.position = Vector3.MoveTowards(this.transform.position, waypoints[waypointIndex],
            speed * 0.009f);

            if (this.transform.position == waypoints[waypointIndex])
            {
                distanceTravelled = Vector3.Distance(this.transform.position, waypoints[waypointIndex]);
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
            */
            #endregion
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
        //if (!hasBeenHit)
        //{
        //    hasBeenHit = true;

        //    brickInfo.armorValue--;

        //    if (brickInfo.armorValue <= 0)
        //    {
        //        hasBeenHit = false;
                AudioManager.instance.PlaySound("BrickExplosion", Vector3.zero);
                DestroyBrick();
        //    }
        //    //else
        //    //{
        //    //    StartCoroutine(HitRecoverDelay(1));
        //    //}

        //}
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
        ScorePoints();

        DespawnBrick();

        SendBreakFeedbacks();

        DropBonusMalus();

        //AudioManager.instance.PlayHitSound(soundName, touchedBrick.Transform.position, touchedBrick.Transform.rotation, hitIntensity);
    }

    private void DespawnBrick()
    {
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
