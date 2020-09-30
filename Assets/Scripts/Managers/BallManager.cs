using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using System.Runtime.CompilerServices;

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

    public bool IsBallPaused { get; set; }

    public event Action OnBallReset;

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
            GameObject instanciatedBall = Instantiate(ballPrefab, -Vector3.one, Quaternion.identity);
            instanciatedBall.SetActive(false);
            return instanciatedBall;
        }
        else if (PhotonNetwork.IsMasterClient)
        {
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

    #region Start methods

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

        if(GameManager.Instance.offlineMode || BallMultiplayerBehaviour.Instance.IsBallOwner)
            BallEventManager.instance.OnCollisionWithRacket -= BallBecomeInPlay;

        StopCoroutine(floatCoroutine);
        canFloat = false;
    }

    private void ResetBall()
    {
        OnBallReset?.Invoke();
    }

    #endregion

    #region Lose Ball

    public void LoseBall()
    {
        if (GameManager.Instance.offlineMode)
        {
            LoseBallLocaly();

            if (LevelManager.instance.currentLevel.level.levelSpec.mandatoryBounce)
            {
                LockWallManager.Instance.EnterProtectionState();
            }
        }
        else
        {
            photonView.RPC("LoseBallLocaly", RpcTarget.All);
        }
    }

    [PunRPC]
    private void LoseBallLocaly()
    {
        if (GetPlayerWhoLostTheBall() == QPlayerManager.instance.LocalPlayerID)
        {
            VibrationManager.instance.VibrateOn("Vibration_Mistake");
        }

        //ScoreManager.Instance.ResetCombo((int)GetPlayerWhoLostTheBall());

        AudioManager.instance.PlaySound("Mistake", Vector3.zero);

        DespawnBallLocaly();

        TargetSelector.SetCurrentTargetPlayer((QPlayer)(int)GetNextPlayerTarget());

        RespawnBall();
    }

    #endregion

    #region Ball Spawn/Despawn

    #region Ball First Spawn
    public void BallFirstSpawn(QPlayer startingPlayer)
    {
        TargetSelector.SetCurrentTargetPlayer(startingPlayer);

        if(GameManager.Instance.offlineMode)
        {
            BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
            BallFisrtSpawnLocally(TargetSelector.GetTargetPlayerPosition() + SpawnOffset);
        }
        else if (TargetSelector.CurrentTargetPlayer == QPlayerManager.instance.LocalPlayerID)
        {
            if (!BallMultiplayerBehaviour.Instance.IsBallOwner)
            {
                BallMultiplayerBehaviour.Instance.OnBallOwnershipAcquisition += AfterOwnershipRequestBallFirstSpawn;
                BallMultiplayerBehaviour.Instance.BecomeBallOwner(BallOwnershipSwitchType.Default);
            }
            else
            {
                GoodOwnerBallFirstSpawn();
            }
                
        }  
    }

    private void AfterOwnershipRequestBallFirstSpawn()
    {
        BallMultiplayerBehaviour.Instance.OnBallOwnershipAcquisition -= AfterOwnershipRequestBallFirstSpawn;
        GoodOwnerBallFirstSpawn();
    }

    private void GoodOwnerBallFirstSpawn()
    {
        BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
        photonView.RPC("BallFisrtSpawnLocally", RpcTarget.All, TargetSelector.GetTargetPlayerPosition() + SpawnOffset);
    }

    [PunRPC]
    private void BallFisrtSpawnLocally(Vector3 spawnLocation)
    {
        SpawnBallLocaly(spawnLocation);

        if (!GameManager.Instance.offlineMode)
            BallMultiplayerBehaviour.Instance.UpdateBallOwnershipBasedStates();

        StartBallFisrtSpawnCoroutine();
    }

    private void StartBallFisrtSpawnCoroutine()
    {
        if (BallApparitionBehaviour != null)
            BallApparitionBehaviour.ResumeLoading();

        BallPhysicBehaviour.StartBallFirstSpawnCoroutine(firstSpawnAnimationDuration);

        BallColorBehaviour.StartBallFirstSpawnCoroutine(firstSpawnAnimationDuration);
    }

    #endregion

    #region Ball Respawn

    public void RespawnBall()
    {
        if (GameManager.Instance.offlineMode)
        {
            BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
            RespawnBallLocaly(TargetSelector.GetTargetPlayerPosition() + SpawnOffset);
        }
        else if(TargetSelector.CurrentTargetPlayer == QPlayerManager.instance.LocalPlayerID)
        {
            if (!BallMultiplayerBehaviour.Instance.IsBallOwner)
            {
                BallMultiplayerBehaviour.Instance.OnBallOwnershipAcquisition += AfterOwnershipRequestRespawnBall;
                BallMultiplayerBehaviour.Instance.BecomeBallOwner(BallOwnershipSwitchType.Default);
            }
            else
            {
                GoodOwnerRespawnBall();
            }
        }
    }

    private void AfterOwnershipRequestRespawnBall()
    {
        BallMultiplayerBehaviour.Instance.OnBallOwnershipAcquisition -= AfterOwnershipRequestRespawnBall;
        GoodOwnerRespawnBall();
    }

    private void GoodOwnerRespawnBall()
    {
        BallEventManager.instance.OnCollisionWithRacket += BallBecomeInPlay;
        photonView.RPC("RespawnBallLocaly", RpcTarget.All, TargetSelector.GetTargetPlayerPosition() + SpawnOffset);
    }

    [PunRPC]
    private void RespawnBallLocaly(Vector3 spawnLocation)
    {
        SpawnBallLocaly(spawnLocation);

        StartFloatCoroutine();
        canFloat = true;
    }

    #endregion

    [PunRPC]
    private void SpawnBallLocaly(Vector3 spawnLocation)
    {
        Ball.transform.position = spawnLocation;
        Ball.SetActive(true);
        BallColorBehaviour.DeactivateTrail();

        ResetBall();

        if (!GameManager.Instance.offlineMode)
            BallMultiplayerBehaviour.Instance.UpdateBallOwnershipBasedStates();
    }

    #region Despawn Ball

    public void DespawnTheBall()
    {
        if (GameManager.Instance.offlineMode)
            DespawnBallLocaly();
        else
            photonView.RPC("DespawnBallLocaly", RpcTarget.All);
    }

    [PunRPC]
    private void DespawnBallLocaly()
    {
        ResetBall();
        Ball.SetActive(false);
    }

    #endregion

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
        if(GameManager.Instance.offlineMode)
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
        else
        {
            if (BallMultiplayerBehaviour.Instance.IsBallOwner)
                return QPlayerManager.instance.OtherPlayerID;
            else
                return QPlayerManager.instance.LocalPlayerID;
        }
    }

    public QPlayer GetPlayerWhoLostTheBall()
    {
        //QPlayer playerWhoLostTheBall;
        //switch (BallInfo.CurrentBallStatus)                                              // A tester
        //{
        //    case BallStatus.HitState:
        //        playerWhoLostTheBall = TargetSelector.GetPreviousTargetPlayer();
        //        break;
        //    case BallStatus.ReturnState:
        //        playerWhoLostTheBall = TargetSelector.CurrentTargetPlayer;
        //        break;
        //    default:
        //        playerWhoLostTheBall = QPlayer.NONE;
        //        break;
        //}
        //return playerWhoLostTheBall;

        if (GameManager.Instance.offlineMode || BallMultiplayerBehaviour.Instance.IsBallOwner)
            return QPlayerManager.instance.LocalPlayerID;
        else
            return QPlayerManager.instance.OtherPlayerID;
    }

    #endregion

    #region  Float Coroutine

    public void StartFloatCoroutine()
    {
        if (floatCoroutine != null)
            StopCoroutine(floatCoroutine);

        floatCoroutine = StartCoroutine(FloatCoroutine());
    }

    public IEnumerator FloatCoroutine()
    {
        BallPhysicBehaviour.SetGravityState(false);

        float t = 0;
        Vector3 startPosition = Ball.transform.position;
        Ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);

        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (!canFloat)
                Ball.transform.position = startPosition;
            else if (!IsBallPaused)
            {
                t += Time.fixedDeltaTime;
                Ball.transform.position = startPosition + new Vector3(0, floatAmplitude * Mathf.Sin(t / floatPeriod * 2 * Mathf.PI), 0);
            }
        }
    }

    #endregion
}
