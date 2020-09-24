using System;
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
    public Vector3 leftHandedSpawnOffset;
    public Vector3 rightHandedSpawnOffset;
    public float firstSpawnAnimationDuration = 1;

    [Header("Float Settings")]
    public float floatAmplitude;
    public float floatPeriod;
    public bool canFloat;

    public bool IsTheLastPlayerWhoHitTheBall { get { return ((int)GetLastPlayerWhoHitTheBall() == 0 && PhotonNetwork.IsMasterClient) || ((int)GetLastPlayerWhoHitTheBall() == 1 && !PhotonNetwork.IsMasterClient); } }


    public Vector3 SpawnOffset
    {
        get => (PlayerSettings.Instance.PlayerDominantHand == PlayerHand.LEFT) ? leftHandedSpawnOffset : rightHandedSpawnOffset;
    }
    public BallColorBehaviour BallColorBehaviour { get; private set; }
    public BallPhysicBehaviour BallPhysicBehaviour { get; private set; }
    public BallInfo BallInfo { get; private set; }
    public ITargetSelector TargetSelector { get; private set; }

    public Coroutine floatCoroutine;

    public BallApparitionBehaviour BallApparitionBehaviour;

    public event Action OnFirstBounce;
    public event Action OnReturnStart;

    public bool IsBallPaused { get; set; }

    private PhotonView photonView;

    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
        IsBallPaused = false;
    }

    #region Instantiation Setup

    public GameObject InstantiateBall()
    {
        //Avoid Akward collision bug during stream Kappa
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), false);

        if (GameManager.Instance.offlineMode)
        {
            //Debug.Log("Instanciate Ball OfflineMode");
            GameObject instanciatedBall = Instantiate(ballPrefab, -Vector3.one, Quaternion.identity);
            instanciatedBall.SetActive(false);
            return instanciatedBall;
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

        Sh_GlobalDissolvePosition.Setup();

        //BallEventManager.instance.OnCollisionWithRacket += GameManager.Instance.StartTheGame;     // Question GD sur le début du timer
    }

    private void SetupBallManager()
    {
        BallColorBehaviour = Ball.GetComponent<BallColorBehaviour>();
        BallPhysicBehaviour = Ball.GetComponent<BallPhysicBehaviour>();
        BallApparitionBehaviour = Ball.GetComponent<BallApparitionBehaviour>();
        TargetSelector = BallPhysicBehaviour.GetComponent<ITargetSelector>();
        BallInfo = Ball.GetComponent<BallInfo>();
        BallInfo.SetupBallInfo();                                                                               // A transformer en start
    }

    #endregion

    #region Ball Manipulation

    public void BallBecomeInPlay(Collision collision)                                                                              //Check util?
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
        BallPhysicBehaviour.ResetGravity();
        BallColorBehaviour.UpdateTrail();

        if(BallMultiplayerBehaviour.Instance.IsBallOwner)
            BallEventManager.instance.OnCollisionWithRacket -= BallBecomeInPlay;

        StopCoroutine(floatCoroutine);
    }

    private void ResetBall()
    {
        BallPhysicBehaviour.ResetBall();
    }

    public void LoseBall()
    {
        int nextPlayerTarget = (int)GetNextPlayerTarget();

        if (GameManager.Instance.offlineMode)
        {
            LoseBallLocaly(nextPlayerTarget);

            if (LevelManager.instance.currentLevel.level.levelSpec.mandatoryBounce)
            {
                LockWallManager.Instance.EnterProtectionState();
            }
        }
        else if (BallMultiplayerBehaviour.Instance.IsBallOwner)
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

        //ScoreManager.Instance.ResetCombo((int)GetPlayerWhoLostTheBall());

        AudioManager.instance.PlaySound("Mistake", Vector3.zero);

        DespawnBallLocaly();

        TargetSelector.SetCurrentTargetPlayer((QPlayer)nextPlayerTargetID);

        SpawnTheBall();
    }

    #endregion

    #region Ball Spawn/Despawn

    public void BallFirstSpawn()
    {
        if(TargetSelector.CurrentTargetPlayer == QPlayerManager.instance.LocalPlayerID)
        {
            if(!BallMultiplayerBehaviour.Instance.IsBallOwner)
            {
                BallMultiplayerBehaviour.Instance.BecomeBallOwner(BallOwnershipSwitchType.Default);
            }
            photonView.RPC("BallFisrtSpawnLocally", RpcTarget.All, TargetSelector.GetTargetPlayerPosition() + SpawnOffset);
        }
    }

    [PunRPC]
    private void BallFisrtSpawnLocally(Vector3 spawnLocation)
    {
        SpawnBallLocaly(spawnLocation);

        BallMultiplayerBehaviour.Instance.UpdateBallOwnershipAssociatedActions();

        StartBallFisrtSpawnCoroutine();
    }

    private void StartBallFisrtSpawnCoroutine()
    {
        if (BallApparitionBehaviour != null)
            BallApparitionBehaviour.ResumeLoading();

        BallPhysicBehaviour.StartBallFirstSpawnCoroutine(firstSpawnAnimationDuration);

        BallColorBehaviour.StartBallFirstSpawnCoroutine(firstSpawnAnimationDuration);
    }

    public void SpawnTheBall()
    {
        if (GameManager.Instance.offlineMode)
            SpawnBallLocaly(TargetSelector.GetTargetPlayerPosition() + SpawnOffset);
        else if (BallMultiplayerBehaviour.Instance.IsBallOwner)
        {
            if (TargetSelector.CurrentTargetPlayer == QPlayerManager.instance.LocalPlayerID)
            {
                BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
                photonView.RPC("SpawnBallLocaly", RpcTarget.All, TargetSelector.GetTargetPlayerPosition() + SpawnOffset);
            }
            else
                photonView.RPC("SwitchOwnerAndSpawnBall", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SpawnBallLocaly(Vector3 spawnLocation)
    {
        Ball.transform.position = spawnLocation;
        Ball.SetActive(true);
        BallColorBehaviour.DeactivateTrail();

        ResetBall();

        //floatCoroutine = StartCoroutine(FloatCoroutine());
    }

    private void SwitchOwnerAndSpawnBall()
    {
        BallMultiplayerBehaviour.Instance.BecomeBallOwner(BallOwnershipSwitchType.Default);

        BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
    }

    public IEnumerator FloatCoroutine()
    {
        float t = 0;
        Vector3 startPosition = Ball.transform.position;
        Ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);

        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (!IsBallPaused && canFloat)
            {
                t += Time.fixedDeltaTime;
                Ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);
            }
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

    #region Pause

    public void PauseBall()
    {
        IsBallPaused = true;
        BallPhysicBehaviour.PauseBallPhysics();
    }

    public void ResumeBall()
    {
        IsBallPaused = false;
        BallPhysicBehaviour.ResumeBallPhysics();
    }

    #endregion

    #region LastPlayerWhoHitTheBall

    public QPlayer GetLastPlayerWhoHitTheBall()
    {
        return BallInfo.LastPlayerWhoHitTheBall;
    }

    private QPlayer GetNextPlayerTarget()
    {
        QPlayer nextPlayerTarget;
        switch (BallInfo.CurrentBallStatus)                                              // A tester
        {
            case BallStatus.HitState:
                nextPlayerTarget = TargetSelector.CurrentTargetPlayer;
                break;
            case BallStatus.ReturnState:
                nextPlayerTarget = TargetSelector.GetPreviousTargetPlayer();
                break;
            default:
                nextPlayerTarget = TargetSelector.CurrentTargetPlayer;
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
                playerWhoLostTheBall = TargetSelector.GetPreviousTargetPlayer();
                break;
            case BallStatus.ReturnState:
                playerWhoLostTheBall = TargetSelector.CurrentTargetPlayer;
                break;
            default:
                playerWhoLostTheBall = QPlayer.NONE;
                break;
        }
        return playerWhoLostTheBall;
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

    #endregion

    #region Physics

    public void SetGlobalSpeedMultiplier(float newValue)
    {
        BallPhysicBehaviour.GlobalSpeedMultiplier = newValue;
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
