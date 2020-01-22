using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Photon.Pun;


public class RacketManager : MonoBehaviour//, //IGrabCaller
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
    [HideInInspector]
    public GameObject localPlayerRacket;
    [HideInInspector]
    public GameObject foreignPlayerRacket;

    public float deltaHitTime = 0.5f; //Valeur A twik
    
    [Header("Racket Grab Position/Rotation")]
    public Vector3 racketOffset = new Vector3(0f, 0.02f, 0.6f);
    public Vector3 racketRotationOffset = new Vector3(0f, 180f, 90f);

    [HideInInspector]
    public bool isEmpowered = false;
    private MeshRenderer localRacketRenderer;
    private MeshRenderer foreignRacketRenderer;

    [Header("Material")]
    public Material[] racketMats;

    private Transform grabPosition;
    private PhotonView photonView;

    public AudioSource sound;
    private bool isPlaying = false;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        grabPosition = racketPrefab.GetComponentInChildren<GrabPositionGetter>().transform;

        SetUpRacketColor();


    }

    public void SetLocalRacket(GameObject localRacket)
    {
        localPlayerRacket = localRacket;

        AssociateRacketWithController();

        localRacketRenderer = localPlayerRacket.GetComponentInChildren<MeshRenderer>();
    }

    public void SetForeignPlayerRacket(GameObject foreignRacket)
    {
        foreignPlayerRacket = foreignRacket;

        foreignRacketRenderer = foreignPlayerRacket.GetComponentInChildren<MeshRenderer>();
    }

    private void AssociateRacketWithController()
    {
        PlayerHand playerMainHand = QPlayerManager.instance.GetMainHand();
        RacketManager.instance.localPlayerRacket.transform.parent = QPlayerManager.instance.GetLocalController(playerMainHand).transform;
        localPlayerRacket.GetComponent<Rigidbody>().useGravity = false;
        localPlayerRacket.GetComponent<Rigidbody>().isKinematic = true;
        localPlayerRacket.transform.localPosition = racketOffset; //grabPosition.localPosition; 
        localPlayerRacket.transform.localEulerAngles = racketRotationOffset; //grabPosition.localRotation;
        //localPlayerRacket.tag = "Racket";
    }

    void SetUpRacketColor()
    {
        racketMats = new Material[3];

        for (int i = 0; i < racketMats.Length; i++)
        {
            racketMats[i] = new Material(Shader.Find("Lightweight Render Pipeline/Lit"));
        }

        racketMats[0].SetColor("_EmissionColor", BrickManager.Instance.colorPresets[0].colorPresets[0].fresnelColors);
        racketMats[1].SetColor("_EmissionColor", BrickManager.Instance.colorPresets[0].colorPresets[1].fresnelColors);
        racketMats[2].SetColor("_EmissionColor", BrickManager.Instance.colorPresets[0].colorPresets[2].fresnelColors);
    }

    public void SwitchRacketColor()
    {
        SwitchLocalRacketColor();
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC("SwitchForeignRacketColor", RpcTarget.Others);
        }
    }

    public void EndSwitchRacketColor()
    {
        EndLocalSwitchColor();
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC("EndForeignSwitchColor", RpcTarget.Others);
        }
    }

    void EndLocalSwitchColor()
    {
        localRacketRenderer.sharedMaterials[1] = racketMats[0];
    }

    [PunRPC]
    void EndForeignSwitchColor()
    {
        foreignRacketRenderer.sharedMaterials[1] = racketMats[0];
    }

    private void SwitchLocalRacketColor()
    {
        localRacketRenderer.sharedMaterials[1] = racketMats[(BallManager.instance.GetBallColorID() + 1) % 2 + 1];
    }

    [PunRPC]
    private void SwitchForeignRacketColor()
    {
        foreignRacketRenderer.sharedMaterials[1] = racketMats[(BallManager.instance.GetBallColorID() + 1) % 2 + 1];
    }

    //////////////////////////////////////////////     Other Methods     //////////////////////////////////////////////

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

    public void EnterEmpoweredState()
    {
        isEmpowered = true;
        sound.Play();
    }

    public void ExitEmpoweredState()
    {
        isEmpowered = false;
        sound.Stop();
    }
}



