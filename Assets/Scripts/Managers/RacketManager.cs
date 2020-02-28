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

    [HideInInspector]
    public bool isEmpowered = false;
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

    private int racketColorID;

    private void Awake()
    {
        instance = this;

        photonView = GetComponent<PhotonView>();

        racketActionType = RacketActionType.NONE;
        //Initialize(RacketActionType.RACKETEMPOWERED);
    }

    public void RacketAction()
    {
        racketAction?.Invoke();
    }

    public void Initialize(RacketActionType newRacketActionType)
    {
        TerminateRacketAction();

        switch(newRacketActionType)
        {
            case RacketActionType.RACKETEMPOWERED:
                racketAction = EmpoweredStateAction;
                BallEventManager.instance.OnBallColorSwitch += SwitchRacketColor;
                SetRacketsColor(BallManager.instance.GetBallColorID() + 1);
                break;
            case RacketActionType.BALLOPPOSITE:
                BallEventManager.instance.OnBallColorSwitch += SwitchRacketColor;
                SetRacketsColor((BallManager.instance.GetBallColorID() + 1) % 2 + 1);
                break;
            default:
                SetRacketsColor(BallManager.instance.GetBallColorID() + 1);
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
        Debug.Log("Set foreignRacket in RacketManager");
        foreignPlayerRacket = foreignRacket;

        foreignRacketRenderer = foreignPlayerRacket.GetComponentInChildren<MeshRenderer>();

        foreignRacketFX = localPlayerRacket.GetComponent<RacketFX>();
    }

    private void AssociateRacketWithController()
    {
        PlayerHand playerMainHand = QPlayerManager.instance.GetMainHand();
        RacketManager.instance.localPlayerRacket.transform.parent = QPlayerManager.instance.GetLocalController(playerMainHand).transform;
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

        for (int i = 0; i < racketMats.Length; i++)
        {
            switch (i)
            {
                case 0:
                    racketMats[i].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors);
                    racketMats[i].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors);
                    break;

                case 1:
                    racketMats[i].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors * 1.5f);
                    racketMats[i].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors * 1.5f);
                    break;

                case 2:
                    racketMats[i].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors * 3);
                    racketMats[i].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors * 3);
                    break;
            }
        }
    }

    public void SetRacketsColor(int newColorID)
    {
        racketColorID = newColorID;
        localRacketRenderer.sharedMaterials = racketMats[racketColorID].racketMaterial;
        if(!GameManager.Instance.offlineMode)
            foreignRacketRenderer.sharedMaterials = racketMats[racketColorID].racketMaterial;

        Debug.Log("SetRacketColor colorID: " + racketColorID);
    }

    public void SwitchRacketColor()                                                                     //Rendre plus propre?
    {
        SwitchLocalRacketColor();

        if (!GameManager.Instance.offlineMode)
        {
            if (foreignPlayerRacket)
                photonView.RPC("SwitchForeignRacketColor", RpcTarget.Others);
            else
                Debug.LogError("NullException : ForeignPlayerRacket not set");
        }
    }

    public void EndSwitchRacketColor()
    {
        SwitchLocalRacketColor();

        if (!GameManager.Instance.offlineMode)
        {
            if (foreignPlayerRacket)
                photonView.RPC("SwitchForeignRacketColor", RpcTarget.Others);
            else
                Debug.LogError("NullException : ForeignPlayerRacket not set");
        }
    }

    private void SwitchLocalRacketColor()
    {
        racketColorID = racketColorID % 2 + 1;
        localRacketRenderer.sharedMaterials = racketMats[racketColorID].racketMaterial;
    }

    [PunRPC]
    private void SwitchForeignRacketColor()
    {
        racketColorID = racketColorID % 2 + 1;
        foreignRacketRenderer.sharedMaterials = racketMats[racketColorID].racketMaterial;
    }

    void EndLocalSwitchColor()                                                                                              // Deprecated
    {
        localRacketRenderer.sharedMaterials = racketMats[(BallManager.instance.GetBallColorID() + 1)].racketMaterial;
    }

    [PunRPC]
    void EndForeignSwitchColor()                                                                                            // Deprecated
    {
        foreignRacketRenderer.sharedMaterials = racketMats[(BallManager.instance.GetBallColorID() + 1)].racketMaterial;
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
        if (isEmpowered)
            ExitEmpoweredState();
        else
            EnterEmpoweredState();
    }

    public void EnterEmpoweredState()
    {
        isEmpowered = true;
        empoweredSound.Play();
        SwitchRacketColor();



        localRacketFX.PlaySwitchColorFX();

        if (!GameManager.Instance.offlineMode)
            foreignRacketFX.PlaySwitchColorFX();

        VibrationManager.instance.VibrateOnRepeat("Vibration_Racket_Empowered", 0.05f);
    }

    public void ExitEmpoweredState()
    {
        isEmpowered = false;
        empoweredSound.Stop();
        EndSwitchRacketColor();

        localRacketFX.StopSwitchColorFX();
        if (!GameManager.Instance.offlineMode)
            foreignRacketFX.StopSwitchColorFX();

        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            VibrationManager.instance.VibrationOff(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right));
        else
            VibrationManager.instance.VibrationOff(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left));
    }
    #endregion
}