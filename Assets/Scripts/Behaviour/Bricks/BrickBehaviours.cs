using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class BrickBehaviours : MonoBehaviourPunCallbacks/*, IPunObservable*/
{
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
        isWaiting = false;
        SetupBrick();
    }


    private void SetupBrick()
    {
        brickInfo = GetComponent<BrickInfo>();
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
}
