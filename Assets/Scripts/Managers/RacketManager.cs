using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Photon.Pun;


public class RacketManager : MonoBehaviour
{
    #region Singleton
    public static RacketManager instance;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public GameObject racketPrefab;
    //[HideInInspector]
    public GameObject localPlayerRacket;
    //[HideInInspector]
    public GameObject foreignPlayerRacket;

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


    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        //SetUpRacketColor();
    }

    #region Set Methods

    public void SetLocalRacket(GameObject localRacket)
    {
        localPlayerRacket = localRacket;

        AssociateRacketWithController();

        localRacketRenderer = localPlayerRacket.GetComponentInChildren<MeshRenderer>();
        LocalRacketPhysicInfo = localPlayerRacket.GetComponent<PhysicInfo>();

        SetUpRacketColor();
    }

    public void SetForeignPlayerRacket(GameObject foreignRacket)
    {
        Debug.Log("Set foreignRacket in RacketManager");
        foreignPlayerRacket = foreignRacket;

        foreignRacketRenderer = foreignPlayerRacket.GetComponentInChildren<MeshRenderer>();
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

    #endregion

    #region RacketColor

    void SetUpRacketColor()
    {
        //racketMats = new RacketMaterials[3];

        for (int i = 0; i < racketMats.Length; i++)
        {
            //racketMats[i].racketMaterial = new Material[3];

            //for (int j = 0; j < racketMats[i].racketMaterial.Length; j++)
            //{
            //    racketMats[i].racketMaterial[j] = new Material(Shader.Find("Lightweight Render Pipeline/Lit"));
            //}


            //racketMats[i].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors);
            //racketMats[i].racketMaterial[1].SetColor("_BaseColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors);

            switch (i)
            {
                case 0:
                    racketMats[i].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors);
                    racketMats[i].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors);
                    //racketMats[i].racketMaterial[2].SetColor("_BaseColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors);
                    //racketMats[i].racketMaterial[0].SetColor("_BaseColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors);
                    //racketMats[i].racketMaterial[1].SetFloat("_Cutoff", 0);
                    //racketMats[i].racketMaterial[1].SetFloat("_Smoothness", 0.261f);
                    //racketMats[i].racketMaterial[1].SetInt("_EMISSION", 1);
                    break;

                case 1:
                    racketMats[i].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors * 1.5f);
                    racketMats[i].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors * 1.5f);
                    //racketMats[i].racketMaterial[2].SetColor("_BaseColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors);
                    //racketMats[i].racketMaterial[0].SetColor("_BaseColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors);
                    //racketMats[i].racketMaterial[1].SetFloat("_Cutoff", 0);
                    //racketMats[i].racketMaterial[1].SetFloat("_Smoothness", 0.424f);
                    //racketMats[i].racketMaterial[1].SetInt("_EMISSION", 1);
                    break;

                case 2:
                    racketMats[i].racketMaterial[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors * 3);
                    racketMats[i].racketMaterial[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors * 3);
                    //racketMats[i].racketMaterial[2].SetColor("_BaseColor", LevelManager.instance.colorPresets[0].colorPresets[i].coreEmissiveColors);
                    //racketMats[i].racketMaterial[0].SetColor("_BaseColor", LevelManager.instance.colorPresets[0].colorPresets[i].fresnelColors);
                    //racketMats[i].racketMaterial[1].SetFloat("_Cutoff", 1);
                    //racketMats[i].racketMaterial[1].SetFloat("_Smoothness", 0.74f);
                    //racketMats[i].racketMaterial[1].SetInt("_EMISSION", 1);
                    break;
            }
        }

        StartCoroutine(StartDelayRacketColor());
    }

    public void SwitchRacketColor()
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
        EndLocalSwitchColor();

        if (!GameManager.Instance.offlineMode)
        {
            if (foreignPlayerRacket)
                photonView.RPC("EndForeignSwitchColor", RpcTarget.Others);
            else
                Debug.LogError("NullException : ForeignPlayerRacket not set");
        }
    }

    private void SwitchLocalRacketColor()
    {
        localRacketRenderer.sharedMaterials = racketMats[(BallManager.instance.GetBallColorID() + 1) % 2 + 1].racketMaterial;
    }

    [PunRPC]
    private void SwitchForeignRacketColor()
    {
        foreignRacketRenderer.sharedMaterials = racketMats[(BallManager.instance.GetBallColorID() + 1) % 2 + 1].racketMaterial;
    }

    void EndLocalSwitchColor()
    {
        localRacketRenderer.sharedMaterials = racketMats[(BallManager.instance.GetBallColorID() + 1)].racketMaterial;
    }

    [PunRPC]
    void EndForeignSwitchColor()
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
    public void EnterEmpoweredState()
    {
        isEmpowered = true;
        empoweredSound.Play();
        SwitchRacketColor();

        VibrationManager.instance.VibrateOnRepeat("Vibration_Racket_Empowered", 0.05f);
    }

    public void ExitEmpoweredState()
    {
        isEmpowered = false;
        empoweredSound.Stop();
        EndSwitchRacketColor();

        if (QPlayerManager.instance.GetMainHand() == PlayerHand.RIGHT)
            VibrationManager.instance.VibrationOff(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right));
        else
            VibrationManager.instance.VibrationOff(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left));
    }

    #endregion

    IEnumerator StartDelayRacketColor()
    {
        yield return new WaitForSeconds(0.01f); // Ceci est très sale ... BEURK !

        EndLocalSwitchColor();
    }
}

[System.Serializable]
public struct RacketMaterials
{
    public Material[] racketMaterial;
}



