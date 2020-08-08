using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Photon.Pun;

[System.Serializable]
public struct RacketMaterials
{
    public Material[] racketMaterial;
}

public enum RacketActionType
{
    NONE,
    RACKETEMPOWERED,
    BALLOPPOSITE
}

public class RacketManager : MonoBehaviour
{
    #region Singleton
    public static RacketManager instance;
    #endregion

    public GameObject racketPrefab;
    //[HideInInspector]
    public GameObject localPlayerRacket;
    //[HideInInspector]
    public GameObject foreignPlayerRacket;

    public RacketFX localRacketFX;
    public RacketFX foreignRacketFX;

    public PhysicInfo LocalRacketPhysicInfo { get; private set; }

    public float deltaHitTime = 0.5f;

    [Header("Racket Grab Position/Rotation")]
    public Vector3 racketOffset = new Vector3(0f, 0.02f, 0.6f);
    public Vector3 racketRotationOffset = new Vector3(0f, 180f, 90f);


    public bool IsEmpowered { get; private set; }
    private MeshRenderer localRacketRenderer;
    private MeshRenderer foreignRacketRenderer;

    //[Header("Material")]
    public Material MaterialPrefab;
    public RacketMaterials[] racketMats;

    private PhotonView photonView;

    public AudioSource empoweredSound;
    private bool isPlaying = false;

    private RacketActionType racketActionType;
    private Action racketAction;
    private Action racketStopAction;

    public int RacketColorID { get; private set; }
    public int ForeignRacketColorID { get; private set; }

    private void Awake()
    {
        instance = this;

        photonView = GetComponent<PhotonView>();

        racketActionType = RacketActionType.NONE;
        IsEmpowered = false;
        //Initialize(RacketActionType.RACKETEMPOWERED);
    }

    public void RacketAction()
    {
        if(PlayerSettings.Instance.SwitchColorInputType == SwitchColorInputType.Hold)
            racketAction?.Invoke();
        else
        {
            EmpoweredStateAction();
        }
    }
    
    public void RacketStopAction()
    {
        if (PlayerSettings.Instance.SwitchColorInputType == SwitchColorInputType.Hold)
            racketStopAction?.Invoke();
    }

    public void Initialize(RacketActionType newRacketActionType)
    {
        TerminateRacketAction();

        //Debug.Log("RacketManager.Initialize( TYPE : " + newRacketActionType + ")");

        switch (newRacketActionType)
        {
            case RacketActionType.NONE:
                SetRacketsColor(BallManager.instance.BallColorBehaviour.GetBallColor());
                break;
            case RacketActionType.RACKETEMPOWERED:
                racketAction = EnterEmpoweredState;
                racketStopAction = ExitEmpoweredState;
                BallEventManager.instance.OnBallColorSwitch += SwitchRacketColor;
                SetRacketsColor(BallManager.instance.GetBallColorID());
                break;
            case RacketActionType.BALLOPPOSITE:
                BallEventManager.instance.OnBallColorSwitch += SwitchRacketColor;
                SetRacketsColor((BallManager.instance.GetBallColorID() + 1) % 2);
                break;
            default:
                SetRacketsColor(BallManager.instance.GetBallColorID());
                break;
        }

        racketActionType = newRacketActionType;
    }

    public void TerminateRacketAction()
    {
        switch (racketActionType)
        {
            case RacketActionType.RACKETEMPOWERED:
                racketAction = null;
                BallEventManager.instance.OnBallColorSwitch -= SwitchRacketColor;
                break;
            case RacketActionType.BALLOPPOSITE:
                BallEventManager.instance.OnBallColorSwitch -= SwitchRacketColor;
                break;
            default:
                break;
        }
    }

    #region Setter
    public void SetLocalRacket(GameObject localRacket)
    {
        localPlayerRacket = localRacket;

        AssociateRacketWithController();

        localRacketRenderer = localPlayerRacket.GetComponentInChildren<MeshRenderer>();
        LocalRacketPhysicInfo = localPlayerRacket.GetComponent<PhysicInfo>();

        SetUpRacketColor();

        localRacketFX = localPlayerRacket.GetComponent<RacketFX>();
    }

    public void SetForeignPlayerRacket(GameObject foreignRacket)
    {
        //Debug.Log("Set foreignRacket in RacketManager");
        foreignPlayerRacket = foreignRacket;

        foreignRacketRenderer = foreignPlayerRacket.GetComponentInChildren<MeshRenderer>();

        foreignRacketFX = localPlayerRacket.GetComponent<RacketFX>();
    }

    private void AssociateRacketWithController()
    {
        RacketManager.instance.localPlayerRacket.transform.parent = QPlayerManager.instance.GetLocalController(PlayerSettings.Instance.PlayerDominantHand).transform;
        localPlayerRacket.GetComponent<Rigidbody>().useGravity = false;
        localPlayerRacket.GetComponent<Rigidbody>().isKinematic = true;
        localPlayerRacket.transform.localPosition = racketOffset;
        localPlayerRacket.transform.localEulerAngles = racketRotationOffset;
        //localPlayerRacket.tag = "Racket";
    }

    public void EnableRackets(bool enabled)
    {
        localPlayerRacket?.SetActive(enabled);
        if(!GameManager.Instance.offlineMode)
            foreignPlayerRacket?.SetActive(enabled);
    }
    #endregion

    #region RacketColor
    void SetUpRacketColor()
    {
        //racketMats = new RacketMaterials[3];

        racketMats[0].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[0].coreEmissiveColors);
        racketMats[0].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[0].coreEmissiveColors);
                    
        racketMats[1].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[1].fresnelColors * 1.5f);
        racketMats[1].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[1].fresnelColors * 1.5f);
                    
        racketMats[2].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors * 3);
        racketMats[2].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors * 3);
    }

    public void SetRacketsColor(int newColorID)
    {
        RacketColorID = newColorID;
        ForeignRacketColorID = newColorID;

        localRacketRenderer.sharedMaterials = racketMats[RacketColorID].racketMaterial;
        if(!GameManager.Instance.offlineMode)
            foreignRacketRenderer.sharedMaterials = racketMats[ForeignRacketColorID].racketMaterial;
    }

    public void SwitchRacketColor()                               
    {
        SwitchLocalRacketColor();

        if (!GameManager.Instance.offlineMode)
        {
            photonView.RPC("SwitchForeignRacketColor", RpcTarget.Others);
        }
    }

    private void SwitchLocalRacketColor()
    {
        RacketColorID = RacketColorID % 2 + 1;
        localRacketRenderer.sharedMaterials = racketMats[RacketColorID].racketMaterial;

        if (IsEmpowered)
            localRacketFX.FXSwitchColorFX();
    }

    [PunRPC]
    private void SwitchForeignRacketColor()
    {
        ForeignRacketColorID = ForeignRacketColorID % 2 + 1;
        foreignRacketRenderer.sharedMaterials = racketMats[ForeignRacketColorID].racketMaterial;
    }
    #endregion

    #region HitEvent
    public void OnHitEvent(GameObject hitObject)                        // Faire Un vrai event?
    {
        StartCoroutine(AfterHitIgnoreCoroutine(hitObject, Time.time));
    }

    private IEnumerator AfterHitIgnoreCoroutine(GameObject hitObject, float lastHitTime)
    {
        Physics.IgnoreCollision(localPlayerRacket.GetComponent<Collider>(), hitObject.GetComponent<Collider>(), true);
        while (Time.time < lastHitTime + deltaHitTime)
        {
            yield return new WaitForFixedUpdate(); // Remplacer par WaitForSeconds
        }
        Physics.IgnoreCollision(localPlayerRacket.GetComponent<Collider>(), hitObject.GetComponent<Collider>(), false);
    }
    #endregion

    #region EmpoweredState Methods
    private void EmpoweredStateAction()
    {
        if (IsEmpowered)
            ExitEmpoweredState();
        else
            EnterEmpoweredState();
    }

    public void EnterEmpoweredState()
    {
        if (IsEmpowered)
            return;

        IsEmpowered = true;

        SwitchRacketColor();

        localRacketFX.PlaySwitchColorFX();

        empoweredSound.Play();

        VibrationManager.instance.VibrateOnRepeat("Vibration_Racket_Empowered", 0.05f);
    }

    public void ExitEmpoweredState()
    {
        if (!IsEmpowered)
            return;

        IsEmpowered = false;

        localRacketFX.StopSwitchColorFX();

        empoweredSound.Stop();
        
        if (PlayerSettings.Instance.PlayerDominantHand == PlayerHand.RIGHT)
            VibrationManager.instance.VibrationOff(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right));
        else
            VibrationManager.instance.VibrationOff(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left));

        Debug.Log("Desempower Switch");
        SwitchRacketColor();
    }
    #endregion
}