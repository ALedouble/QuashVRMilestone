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
    public GameObject localPlayerRacket;
    public float deltaHitTime = 0.5f; //Valeur A twik
    public bool isEmpowered = false;

    public Material[] racketMats;

    private Transform grabPosition;

    private void Start()
    {
        grabPosition = racketPrefab.GetComponentInChildren<GrabPositionGetter>().transform;

        SetUpRacketColor();
    }

    public void SetLocalRacket(GameObject localRacket)
    {
        localPlayerRacket = localRacket;

        AssociateRacketWithController();
    }

    private void AssociateRacketWithController()
    {
        PlayerHand playerMainHand = QPlayerManager.instance.GetMainHand();
        RacketManager.instance.localPlayerRacket.transform.parent = QPlayerManager.instance.GetLocalController(playerMainHand).transform;
        localPlayerRacket.GetComponent<Rigidbody>().useGravity = false;
        localPlayerRacket.GetComponent<Rigidbody>().isKinematic = true;
        localPlayerRacket.transform.localPosition = new Vector3(0f, 0.02f, 0.6f); //grabPosition.localPosition; 
        localPlayerRacket.transform.localEulerAngles = new Vector3(0f, 180f, 90f); //grabPosition.localRotation;
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

    void SwitchRacketColor()
    {
        
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
    }

    public void ExitEmpoweredState()
    {
        isEmpowered = false;
    }
}



