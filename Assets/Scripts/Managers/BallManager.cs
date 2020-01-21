using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;


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

    private PhotonView photonView;
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

    public void LoseBall()
    {
        if(PhotonNetwork.OfflineMode)
        {
            DespawnBall();
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DespawnBall", RpcTarget.All);
        }

        if (GameManager.Instance.GetGameStatus())
        {
            if (PhotonNetwork.OfflineMode)
            {
                SpawnBall();
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SpawnBall", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void SpawnBall()
    {
        ResetBall();
        ball.transform.position = targetSelector.GetTargetPlayerPosition() + spawnOffset;
        ball.SetActive(true);

        BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
        floatCoroutine = StartCoroutine(FloatCoroutine());
        isSpawned = true;
    }

    [PunRPC]
    public void DespawnBall()
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
        if(PhotonNetwork.IsMasterClient)
        {
            SetBallInPlay();
        }
        else
        {
            photonView.RPC("SetBallInPlay", RpcTarget.All);
        }
    }

    private void SetBallInPlay()
    {
        isInPlay = true;
        ballPhysicBehaviour.ApplyBaseGravity();

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
