﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;


public class BallManager : MonoBehaviour
{
    #region Singleton
    public static BallManager instance;
    #endregion

    

    public bool isBallInstatiated;
    public GameObject ballPrefab;
    public GameObject Ball { get; private set; }

    [Header("Spawn Settings")]
    public Vector3 spawnOffset;
    public int spawnColorID = 0;
    public float firstSpawnAnimationDuration = 1;

    [Header("Float Settings")]
    public float floatAmplitude;
    public float floatPeriod;

    [Header("Reset Settings")]
    public float delayBeforeReset;

    public bool IsTheLastPlayerWhoHitTheBall { get { return ((int)GetLastPlayerWhoHitTheBall() == 0 && PhotonNetwork.IsMasterClient) || ((int)GetLastPlayerWhoHitTheBall() == 1 && !PhotonNetwork.IsMasterClient); } }
   
    public BallColorBehaviour BallColorBehaviour { get; private set; }
    public BallPhysicBehaviour BallPhysicBehaviour { get; private set; }
    public PhysicInfo BallPhysicInfo { get; private set; }
    public BallInfo BallInfo { get; private set; }
    public ITargetSelector TargetSelector { get; private set; }

    private Coroutine floatCoroutine;
    private Coroutine resetCoroutine;

    public event Action OnFirstBounce;
    public event Action OnReturnStart;

    private PhotonView photonView;

    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public GameObject InstantiateBall()
    {
        if (GameManager.Instance.offlineMode)
        {
            //Debug.Log("Instanciate Ball OfflineMode");
            GameObject returnBall = Instantiate(ballPrefab, -Vector3.one, Quaternion.identity);
            returnBall.SetActive(false);
            return returnBall;
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("Instanciate Ball MasterClient");
            return PhotonNetwork.Instantiate(ballPrefab.name, -Vector3.one, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Wrong use of BallManagerInstantiateBall");
            return null;
        }
    }

    public void SetupBall(GameObject newBall)
    {
        if (newBall.tag != "Ball")
            return;

        Ball = newBall;
        SetupBallManager();

        //ballColorBehaviour.SetBallColor(spawnColorID);                                            // Ball Starting Color in levelManager?

        Sh_GlobalDissolvePosition.Setup();

        //BallEventManager.instance.OnCollisionWithRacket += GameManager.Instance.StartTheGame;     // Question GD sur le début du timer
    }

    private void SetupBallManager()
    {
        BallColorBehaviour = Ball.GetComponent<BallColorBehaviour>();
        BallPhysicBehaviour = Ball.GetComponent<BallPhysicBehaviour>();
        BallPhysicInfo = Ball.GetComponent<PhysicInfo>();
        
        TargetSelector = BallPhysicBehaviour.GetTargetSelector();
        BallInfo = Ball.GetComponent<BallInfo>();
        BallInfo.SetupBallInfo();                                                                               // A transformer en start
    }


    public void BallBecomeInPlay()                                                                              //Check util?
    {
        if(GameManager.Instance.offlineMode)
            SetBallInPlay();
        else
        {
            photonView.RPC("SetBallInPlay", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SetBallInPlay()
    {
        BallPhysicBehaviour.ApplyBaseGravity();
        BallColorBehaviour.UpdateTrail();
        BallEventManager.instance.OnCollisionWithRacket -= BallBecomeInPlay;
        StopCoroutine(floatCoroutine);
    }

    private void ResetBall()                                                                                        // ???
    {
        BallPhysicBehaviour.ResetBall();
    }

    #region Ball Manipulation
    public void BallFirstSpawn()
    {
        //Debug.Log("BallFirstSpawn SpawnBallLocally!");
        SpawnBallLocaly();
        
        BallPhysicBehaviour.StartBallFirstSpawnCoroutine(firstSpawnAnimationDuration);

        BallColorBehaviour.StartBallFirstSpawnCoroutine(firstSpawnAnimationDuration);

        // Ajouter le Timer
    }

    public void SpawnTheBall()
    {
        if (GameManager.Instance.offlineMode)
            SpawnBallLocaly();
        else if (PhotonNetwork.IsMasterClient)
            photonView.RPC("SpawnBallLocaly", RpcTarget.All);
    }

    [PunRPC]
    private void SpawnBallLocaly()
    {
        Ball.transform.position = TargetSelector.GetTargetPlayerPosition() + spawnOffset;
        Ball.SetActive(true);
        BallColorBehaviour.DeactivateTrail();

        ResetBall();

        BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
        floatCoroutine = StartCoroutine(FloatCoroutine());
    }

    public IEnumerator FloatCoroutine()
    {
        float t = 0;
        Vector3 startPosition = Ball.transform.position;
        Ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);

        while (true)
        {
            yield return new WaitForFixedUpdate();

            t += Time.fixedDeltaTime;
            Ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);
        }
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
        Ball.SetActive(false);
    }
    #endregion

    #region Gameplay
    public void LoseBall()
    {
        int nextPlayerTarget = (int)GetNextPlayerTarget();

        if (GameManager.Instance.offlineMode)
        {
            LoseBallLocaly(nextPlayerTarget);
        }
        else if (PhotonNetwork.IsMasterClient)
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

        TargetSelector.SetCurrentTarget((QPlayer)nextPlayerTargetID);
        SpawnBallLocaly();
    }

    public QPlayer GetLastPlayerWhoHitTheBall()
    {
        return BallPhysicBehaviour.GetLastPlayerWhoHitTheBall();
    }

    private QPlayer GetNextPlayerTarget()
    {
        QPlayer nextPlayerTarget;
        switch (BallInfo.CurrentBallStatus)                                              // A tester
        {
            case BallStatus.HitState:
                nextPlayerTarget = TargetSelector.GetCurrentTarget();
                break;
            case BallStatus.ReturnState:
                nextPlayerTarget = TargetSelector.GetPreviousTarget();
                break;
            default:
                nextPlayerTarget = TargetSelector.GetCurrentTarget();
                break;
        }
        return nextPlayerTarget;
    }

    public QPlayer GetPlayerWhoLostTheBall()
    {
        QPlayer playerWhoLostTheBall;
        switch (BallInfo.CurrentBallStatus)                                              // A tester
        {
            case BallStatus.HitState:
                playerWhoLostTheBall = TargetSelector.GetPreviousTarget();
                break;
            case BallStatus.ReturnState:
                playerWhoLostTheBall = TargetSelector.GetCurrentTarget();
                break;
            default:
                playerWhoLostTheBall = QPlayer.NONE;
                break;
        }
        return playerWhoLostTheBall;
    }

    public void StartBallResetCountdown()
    {
        Debug.Log("StartBallResetCountdown");
        if(GameManager.Instance.offlineMode || PhotonNetwork.IsMasterClient)
        {
            BallEventManager.instance.OnCollisionExitWithFloor += StopBallResetCountdown;
            BallEventManager.instance.OnCollisionWithBackWall += StopBallResetCountdown;

            Debug.Log("StartBallResetCountdown : Start Coroutine!");
            resetCoroutine = StartCoroutine(BallResetCoroutine());
        }
    }

    private void StopBallResetCountdown()
    {
        BallEventManager.instance.OnCollisionExitWithFloor -= StopBallResetCountdown;
        BallEventManager.instance.OnCollisionWithBackWall -= StopBallResetCountdown;

        StopCoroutine(resetCoroutine);
    }

    private IEnumerator BallResetCoroutine()
    {
        Debug.Log("BallResetCoroutine");
        float resetTime = Time.time + delayBeforeReset;
        while (Time.time < resetTime)
        {
            yield return new WaitForFixedUpdate();
        }

        BallEventManager.instance.OnCollisionWithBackWall -= StopBallResetCountdown;
        LoseBall();
        Debug.Log("BallResetCoroutine: LoseBall");
    }
    #endregion

    #region Color
    public int GetBallColorID()
    {
        return BallColorBehaviour.GetBallColor();
    }

    public Material GetCurrentMaterial()
    {
        return BallColorBehaviour.GetCurrentMaterial();
    }

    public Material[] GetBallMaterials()
    {
        return BallColorBehaviour.GetBallMaterials();
    }

    public int GetSpawnColorID()
    {
        return spawnColorID;
    }
    #endregion

    #region Physics
    public void SetGlobalSpeedMultiplier(float newValue)
    {
        BallPhysicBehaviour.SetGlobalSpeedMultiplier(newValue);
    }
    #endregion

    public void SendOnFirstBounceEvent()
    {
        OnFirstBounce?.Invoke();
    }

    public void SendOnReturnStart()
    {
        OnReturnStart?.Invoke();
    }
}
