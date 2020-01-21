using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallManager : MonoBehaviour
{
    #region Singleton
    public static BallManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject ball;

    [Header("Spawn Settings")]
    public Vector3 spawnOffset;

    [Header("Float Settings")]
    public float floatAmplitude;
    public float floatPeriod;

    private BallColorBehaviour ballColorBehaviour;
    private BallPhysicBehaviour ballPhysicBehaviour;
    private PhysicInfo ballPhysicInfo;
    private ITargetSelector targetSelector;
    
    private bool isInPlay;
    private bool isSpawned;
    private Coroutine floatCoroutine;

    private void Start()
    {
        ballColorBehaviour = ball.GetComponent<BallColorBehaviour>();
        ballPhysicBehaviour = ball.GetComponent<BallPhysicBehaviour>();
        ballPhysicInfo = ball.GetComponent<PhysicInfo>();
        targetSelector = ball.GetComponent<ITargetSelector>(); 

        isInPlay = false;
        isSpawned = false;
    }


    #region Gameplay

    public void SpawnBall()
    {
        ResetBall();
        ball.transform.position = targetSelector.GetTargetPlayerPosition() + spawnOffset;
        ball.SetActive(true);

        BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
        floatCoroutine = StartCoroutine(FloatCoroutine());
        isSpawned = true;
    }

    public void DespawnBall()
    {
        ResetBall();
        ball.SetActive(false);

        if(GameManager.Instance.GetGameStatus())
        {
            //Remettre le spawn
        }

        isInPlay = false;
    }

    private void ResetBall()
    {
        ballPhysicBehaviour.ResetBall();
    }

    #endregion

    #region Color
    public BallColorBehaviour GetBallColorBehaviour()
    {
        return ballColorBehaviour;
    }

    public int GetBallColorID()
    {
        return ballColorBehaviour.GetBallColor();
    }

    public Material GetCurrentMaterial()
    {
        return ballColorBehaviour.GetCurrentMaterial();
    }

    public Material[] GetBallMaterials()
    {
        return ballColorBehaviour.GetBallMaterials();
    }

    #endregion

    #region Physics

    public BallPhysicBehaviour GetBallPhysicsBehaviour()
    {
        return GetBallPhysicsBehaviour();
    }

    public PhysicInfo GetBallPhysicInfo()
    {
        return ballPhysicInfo;
    }

    #endregion

    #region Utility

    public QPlayer GetLastPlayerWhoHitTheBall()
    {
        return ballPhysicBehaviour.GetLastPlayerWhoHitTheBall();
    }

    public void TransferEmpowerement()
    {
        if (ballColorBehaviour.colorSwitchTrigerType == ColorSwitchTrigerType.WALLBASED)
            ballColorBehaviour.TransferEmpowerement();
    }

    public bool GetBallStatus()
    {
        return isInPlay;
    }

    public void BallBecomeInPlay()
    {
        isInPlay = true;
        ballPhysicBehaviour.ApplyBaseGravity();
        StopCoroutine(floatCoroutine);
    }

    public IEnumerator FloatCoroutine()
    {
        float t = 0;
        Vector3 startPosition = ball.transform.position;
        ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);


        while (true)
        {
            yield return new WaitForFixedUpdate();

            t += Time.fixedDeltaTime;
            ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);
        }
    }

    #endregion

}
