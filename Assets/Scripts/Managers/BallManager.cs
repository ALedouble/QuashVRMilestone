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

    
    public BallColorBehaviour BallColorBehaviour { get; private set; }
    public BallPhysicBehaviour BallPhysicBehaviour { get; private set; }
    public PhysicInfo BallPhysicInfo { get; private set; }
    public BallInfo BallInfo { get; private set; }
    public ITargetSelector TargetSelector { get; private set; }

    private bool isInPlay;
    private bool isSpawned;
    private Coroutine floatCoroutine;
    private Coroutine resetCoroutine;

    private PhotonView photonView;

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

    public void InstantiateBall()
    {
        if (GameManager.Instance.offlineMode)
        {
            Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }

    public void InitializeBall()                                                                                                     //Suppr
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
    private void SetupOnlineBall()                                                              // Suppr
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        SetupBall();
    }

    public void SetupBall(GameObject newBall)
    {
        if (newBall.tag != "Ball")
            return;

        ball = newBall;
        SetupBallManager();

        //ballColorBehaviour.SetBallColor(spawnColorID);                                            // Ball Starting Color in levelManager?

        ball.SetActive(false);

        Sh_GlobalDissolvePosition.Setup();

        //BallEventManager.instance.OnCollisionWithRacket += GameManager.Instance.StartTheGame;     // Question GD sur le début du timer
    }

    public void SetupBall()                                                                         // Suppr
    {
        if(!ball)
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
        }

        SetupBallManager();
        //ballColorBehaviour.SetBallColor(spawnColorID);                                            // Ball Starting Color in levelManager?
        
        ball.SetActive(false);
        //ballColorBehaviour.DeactivateTrail();                                                     // Inutil
        
        Sh_GlobalDissolvePosition.Setup();

        //BallEventManager.instance.OnCollisionWithRacket += GameManager.Instance.StartTheGame;     // Question GD sur le début du timer
    }

    private void SetupBallManager()
    {
        BallColorBehaviour = ball.GetComponent<BallColorBehaviour>();
        BallPhysicBehaviour = ball.GetComponent<BallPhysicBehaviour>();
        BallPhysicInfo = ball.GetComponent<PhysicInfo>();
        
        TargetSelector = BallPhysicBehaviour.GetTargetSelector();
        BallInfo = ball.GetComponent<BallInfo>();
        BallInfo.SetupBallInfo();                                                                               // A transformer en start
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
        BallPhysicBehaviour.ApplyBaseGravity();
        BallColorBehaviour.UpdateTrail();
        BallEventManager.instance.OnCollisionWithRacket -= BallBecomeInPlay;
        StopCoroutine(floatCoroutine);
    }

    private void ResetBall()                                                                                        // ???
    {
        BallPhysicBehaviour.ResetBall();
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

        TargetSelector.SetCurrentTarget((QPlayer)nextPlayerTargetID);
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
        ball.transform.position = TargetSelector.GetTargetPlayerPosition() + spawnOffset;
        BallColorBehaviour.DeactivateTrail();
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

        BallEventManager.instance.OnCollisionWithBackWall -= StopBallResetCountdown;
        LoseBall();
    }
    #endregion

    #region Color
    public BallColorBehaviour GetBallColorBehaviour()
    {
        return BallColorBehaviour;
    }

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

    public BallPhysicBehaviour GetBallPhysicsBehaviour()
    {
        return GetBallPhysicsBehaviour();
    }

    public PhysicInfo GetBallPhysicInfo()
    {
        return BallPhysicInfo;
    }

    public void SetGlobalSpeedMultiplier(float newValue)
    {
        BallPhysicBehaviour.SetGlobalSpeedMultiplier(newValue);
    }

    #endregion

    #region Utility

    public QPlayer GetLastPlayerWhoHitTheBall()
    {
        return BallPhysicBehaviour.GetLastPlayerWhoHitTheBall();
    }

    public void TransferEmpowerement()
    {
        if (BallColorBehaviour.colorSwitchTrigerType == ColorSwitchTrigerType.WALLBASED)
            BallColorBehaviour.TransferEmpowerement();
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
