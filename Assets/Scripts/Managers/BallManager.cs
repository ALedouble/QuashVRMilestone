﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;


public class BallManager : MonoBehaviour
{
    #region Singleton
    public static BallManager instance;
    #endregion

    public int spawnColorID = 0;

    public bool isBallInstatiated;
    public GameObject ballPrefab;
    public GameObject ball;

    [Header("Spawn Settings")]
    public Vector3 spawnOffset;

    [Header("Float Settings")]
    public float floatAmplitude;
    public float floatPeriod;

    [Header("Reset Settings")]
    public float delayBeforeReset;

    public bool IsTheLastPlayerWhoHitTheBall { get { return ((int)GetLastPlayerWhoHitTheBall() == 0 && PhotonNetwork.IsMasterClient) || ((int)GetLastPlayerWhoHitTheBall() == 1 && !PhotonNetwork.IsMasterClient); } }

    private PhotonView photonView;
    private BallColorBehaviour ballColorBehaviour;
    private BallPhysicBehaviour ballPhysicBehaviour;
    private PhysicInfo ballPhysicInfo;
    private BallInfo ballInfo;
    private ITargetSelector targetSelector;
    
    private bool isInPlay;
    private bool isSpawned;
    private Coroutine floatCoroutine;
    private Coroutine resetCoroutine;


    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        isInPlay = false;
        isSpawned = false;

        //SetupBall();
    }

    public void InitializeBall()
    {
        if (isBallInstatiated)
        {
            if (GameManager.Instance.offlineMode)
            {
                ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, Quaternion.identity);
            }
        }

        //if (GameManager.Instance.offlineMode)
        //{
        //    SetupBall();
        //}
        //else if (PhotonNetwork.IsMasterClient)
        //{
        //    photonView.RPC("SetupOnlineBall", RpcTarget.All);
        //}
    }

    [PunRPC]
    private void SetupOnlineBall()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        SetupBall();
    }

    public void SetupBall()
    {
        if(!ball)
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
        }

        SetupBallManager();
        ballColorBehaviour.SetBallColor(spawnColorID);
        ball.SetActive(false);
        ballColorBehaviour.DeactivateTrail();
        Sh_GlobalDissolvePosition.Setup();

        BallEventManager.instance.OnCollisionWithRacket += GameManager.Instance.StartTheGame;
    }

    private void SetupBallManager()
    {
        ballColorBehaviour = ball.GetComponent<BallColorBehaviour>();
        ballPhysicBehaviour = ball.GetComponent<BallPhysicBehaviour>();
        ballPhysicInfo = ball.GetComponent<PhysicInfo>();
        targetSelector = ballPhysicBehaviour?.GetTargetSelector();
        ballInfo = ball.GetComponent<BallInfo>();
        ballInfo.SetupBallInfo();
    }

    #region Gameplay

    public void LoseBall()
    {
        int nextPlayerTarget = (int)GetNextPlayerTarget();

        if (GameManager.Instance.offlineMode)
        {
            LoseBallLocaly(nextPlayerTarget);
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("LoseBallLocaly", RpcTarget.All, nextPlayerTarget);
        }
    }

    [PunRPC]
    private void LoseBallLocaly(int nextPlayerTargetID)
    {
        if (GetPlayerWhoLostTheBall() == QPlayerManager.instance.LocalPlayerID)
        {
            VibrationManager.instance.VibrateOn("Vibration_Mistake");
        }

        AudioManager.instance.PlaySound("Mistake", Vector3.zero);

        DespawnBallLocaly();

        targetSelector.SetCurrentTarget((QPlayer)nextPlayerTargetID);
        SpawnBallLocaly();
    }

    public void SpawnTheBall()
    {
        if (GameManager.Instance.offlineMode)
            SpawnBallLocaly();
        else /*if (PhotonNetwork.IsMasterClient)*/
            photonView.RPC("SpawnBallLocaly", RpcTarget.All);
    }

    [PunRPC]
    private void SpawnBallLocaly()
    {
        ball.transform.position = targetSelector.GetTargetPlayerPosition() + spawnOffset;
        ballColorBehaviour.DeactivateTrail();
        ball.SetActive(true);

        ResetBall();

        BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
        floatCoroutine = StartCoroutine(FloatCoroutine());
        isSpawned = true;
    }

    public void DespawnTheBall()
    {
        if (GameManager.Instance.offlineMode)
            DespawnBallLocaly();
        else if (PhotonNetwork.IsMasterClient)
            photonView.RPC("DespawnBallLocaly", RpcTarget.All);
    }

    [PunRPC]
    private void DespawnBallLocaly()
    {
        ResetBall();
        ball.SetActive(false);

        isInPlay = false;
        isSpawned = false;
    }

    private void ResetBall()
    {
        ballPhysicBehaviour.ResetBall();
    }

    
    public void BallBecomeInPlay()
    {
        //if(GameManager.Instance.offlineMode)
            SetBallInPlay();
        //else
        //{
        //    photonView.RPC("SetBallInPlay", RpcTarget.All);
        //}
    }

    [PunRPC]
    private void SetBallInPlay()
    {
        isInPlay = true;
        ballPhysicBehaviour.ApplyBaseGravity();
        ballColorBehaviour.UpdateTrail();
        BallEventManager.instance.OnCollisionWithRacket -= BallBecomeInPlay;
        StopCoroutine(floatCoroutine);
    }

    private QPlayer GetNextPlayerTarget()
    {
        QPlayer nextPlayerTarget;
        switch (ballInfo.CurrentBallStatus)                                              // A tester
        {
            case BallStatus.HitState:
                nextPlayerTarget = targetSelector.GetCurrentTarget();
                break;
            case BallStatus.ReturnState:
                nextPlayerTarget = targetSelector.GetPreviousTarget();
                break;
            default:
                nextPlayerTarget = targetSelector.GetCurrentTarget();
                break;
        }
        return nextPlayerTarget;
    }

    private QPlayer GetPlayerWhoLostTheBall()
    {
        QPlayer playerWhoLostTheBall;
        switch (ballInfo.CurrentBallStatus)                                              // A tester
        {
            case BallStatus.HitState:
                playerWhoLostTheBall = targetSelector.GetPreviousTarget();
                break;
            case BallStatus.ReturnState:
                playerWhoLostTheBall = targetSelector.GetCurrentTarget();
                break;
            default:
                playerWhoLostTheBall = QPlayer.NONE;
                break;
        }
        return playerWhoLostTheBall;
    }

    public void StartBallResetCountdown()
    {
        BallEventManager.instance.OnCollisionExitWithFloor += StopBallResetCountdown;
        BallEventManager.instance.OnCollisionWithBackWall += StopBallResetCountdown;

        resetCoroutine = StartCoroutine(BallResetCoroutine());
    }

    private void StopBallResetCountdown()
    {
        BallEventManager.instance.OnCollisionExitWithFloor -= StopBallResetCountdown;
        BallEventManager.instance.OnCollisionWithBackWall -= StopBallResetCountdown;

        StopCoroutine(resetCoroutine);
    }

    private IEnumerator BallResetCoroutine()
    {
        float resetTime = Time.time + delayBeforeReset;
        while (Time.time < resetTime)
        {
            yield return new WaitForFixedUpdate();
        }
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

    public void SetGlobalSpeedMultiplier(float newValue)
    {
        ballPhysicBehaviour.SetGlobalSpeedMultiplier(newValue);
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

    public int GetSpawnColorID()
    {
        return spawnColorID;
    }
}
