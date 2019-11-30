using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehaviours : MonoBehaviour, IBrick
{
    [Header("Score Modifier")]
    public int scoreValue;

    [Header("Armor")]
    [Tooltip("How many hit BEFORE the next one kill it")]
    public int armorPoints = 1;


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
    [SerializeField] bool isBonus;
    [SerializeField] bool isMalus;

    [Header("Color ID")]
    [SerializeField] int colorID;

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
        this.transform.position = Vector3.SmoothDamp(this.transform.position, waypoints[waypointIndex], ref refVector, smoothTime,
            speed);

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
        armorPoints--;

        if (armorPoints <= 0)
        {
            PoolManager.instance.SpawnFromPool("CubeImpactFX", transform.position, Quaternion.identity); //Spawn destroy FX
        }

        //other case scenario
    }

    public BrickInfo GetBrickInfo()
    {
        return new BrickInfo(scoreValue, armorPoints,ColorID, IsBonus, isMalus, transform);
    }
}

public class BrickInfo
{
    int scoreValue;
    int armorValue;
    int colorID;
    bool isBonus;
    bool isMalus;
    Transform transform;

    public BrickInfo(int p_score,int p_armor,int p_colorID,bool p_isBonus, bool p_isMalus, Transform p_transform)
    {
        ScoreValue = p_score;
        ArmorValue = p_armor;
        ColorID = p_colorID;
        IsBonus = p_isBonus;
        IsMalus = p_isMalus;
        Transform = p_transform;
    }

    public int ScoreValue { get => scoreValue; private set => scoreValue = value; }
    public int ArmorValue { get => armorValue; private set => armorValue = value; }
    public int ColorID { get => colorID; private set => colorID = value; }
    public bool IsBonus { get => isBonus; private set => isBonus = value; }
    public bool IsMalus { get => isMalus; private set => isMalus = value; }
    public Transform Transform { get => transform; private set => transform = value; }
}
