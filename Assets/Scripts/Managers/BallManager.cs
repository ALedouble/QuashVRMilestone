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

    public bool isBallInstatiated;
    public GameObject ballPrefab;
    public GameObject ball;

    [Header("Spawn Settings")]
    public Vector3 spawnOffset;

    [Header("Float Settings")]
    public float floatAmplitude;
    public float floatPeriod;

    private PhotonView photonView;
    private BallColorBehaviour ballColorBehaviour;
    private BallPhysicBehaviour ballPhysicBehaviour;
    private PhysicInfo ballPhysicInfo;
    private ITargetSelector targetSelector;
    
    private bool isInPlay;
    private bool isSpawned;
    private Coroutine floatCoroutine;

    public bool IsTheLastPlayerWhoHitTheBall { get { return ((int)GetLastPlayerWhoHitTheBall() == 0 && PhotonNetwork.IsMasterClient) || ((int)GetLastPlayerWhoHitTheBall() == 1 && !PhotonNetwork.IsMasterClient); } }


    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        isInPlay = false;
        isSpawned = false;
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

        if (GameManager.Instance.offlineMode)
        {
            SetupBall();
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetupOnlineBall", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SetupOnlineBall()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        SetupBall();
    }

    private void SetupBall()
    {
        SetupBallManager();
        ball.SetActive(false);
        Sh_GlobalDissolvePosition.Setup();

        BallEventManager.instance.OnCollisionWithRacket += GameManager.Instance.StartTheGame;
        BallManager.instance.SpawnTheBall();
    }

    private void SetupBallManager()
    {
        ballColorBehaviour = ball.GetComponent<BallColorBehaviour>();
        ballPhysicBehaviour = ball.GetComponent<BallPhysicBehaviour>();
        ballPhysicInfo = ball.GetComponent<PhysicInfo>();
        targetSelector = ball.GetComponent<ITargetSelector>();
    }

    #region Gameplay

    public void LoseBall()
    {
        if(GameManager.Instance.offlineMode)
        {
            LoseBallLocaly();
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("LoseBallLocaly", RpcTarget.All);
        }
    }

    [PunRPC]
    private void LoseBallLocaly()
    {
        DespawnBallLocaly();
        targetSelector.SwitchTarget();
        SpawnBallLocaly();

        if(IsTheLastPlayerWhoHitTheBall)
        {
            VibrationManager.instance.VibrateOn("Vibration_Mistake");
        }
        
        AudioManager.instance.PlaySound("Mistake", Vector3.zero);
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
