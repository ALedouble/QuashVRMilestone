﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public enum ColorSwitchTrigerType
{
    RACKETBASED,
    WALLBASED
}

public class BallColorBehaviour : MonoBehaviour//, IPunObservable
{
    public ColorSwitchTrigerType colorSwitchTrigerType = ColorSwitchTrigerType.WALLBASED;

    [Header("Color Settings")]
    public PresetScriptable[] colorPresets;
    private Material[] materials;
    private Material[] wallMats;

    [Header("Trail Settings")]
    public GameObject[] trails;


    private int colorID = 1;

    private bool isEmpowered;

    private PhotonView photonView;
    private Renderer myRenderer;






    private void Start()
    {
        photonView = PhotonView.Get(this);
        myRenderer = gameObject.GetComponent<Renderer>();

        materials = new Material[2];
        wallMats = new Material[2];

        if (GameManager.Instance.offlineMode)
        {
            SetupColors();
        }
        else /*if(PhotonNetwork.IsMasterClient)*/
        {
            photonView.RPC("SetupColors", RpcTarget.AllBuffered);
        }

        InitializeSwitchColor();
    }

    public int GetBallColor()
    {
        return colorID;
    }

    public Material GetCurrentMaterial()
    {
        return materials[colorID];
    }

    public Material[] GetBallMaterials()
    {
        return materials;
    }

    public void SetBallColor(int colorID)
    {
        this.colorID = colorID;
        myRenderer.material = materials[colorID];
    }

    private void InitializeSwitchColor()
    {
        if (colorSwitchTrigerType == ColorSwitchTrigerType.WALLBASED)
        {
            BallEventManager.instance.OnCollisionWithFrontWall += WallBaseSwitchColor;
            BallEventManager.instance.OnCollisionWithBrick += WallBaseSwitchColor;
        }
        else if (colorSwitchTrigerType == ColorSwitchTrigerType.RACKETBASED)
        {
            BallEventManager.instance.OnCollisionWithRacket += RacketBaseSwitchColor;
        }
    }

    [PunRPC]
    public void TransferEmpowerement()
    {
        if (RacketManager.instance.isEmpowered)
        {
            if (PhotonNetwork.OfflineMode)
            {
                BallBecomeEmpowered();
            }
            else
            {
                photonView.RPC("BallBecomeEmpowered", RpcTarget.All);
            }

            //RacketManager.instance.ExitEmpoweredState();
        }
    }

    [PunRPC]
    private void BallBecomeEmpowered()
    {
        isEmpowered = true;
    }

    private void WallBaseSwitchColor()
    {
        if (isEmpowered)
        {
            isEmpowered = false;
            SwitchColor();
        }
    }

    private void RacketBaseSwitchColor()
    {
        if (RacketManager.instance.isEmpowered)
        {
            if (PhotonNetwork.OfflineMode)
            {
                SwitchColor();
            }
            else
            {
                photonView.RPC("SwitchColor", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void SwitchColor()
    {
        Debug.Log("ColorID : " + colorID);

        colorID = (colorID + 1) % materials.Length;
        myRenderer.sharedMaterial = materials[colorID];

        trails[colorID].SetActive(true);
        trails[((colorID - 1) % trails.Length + trails.Length) % trails.Length].SetActive(false);       // Prevent negative value of modulo

        for (int i = 0; i < LevelManager.instance.allMeshes.Length; i++)
        {
            LevelManager.instance.allMeshes[i].sharedMaterial = wallMats[colorID];
        }

    }

    [PunRPC]
    private void SetupColors()
    {
        SetupMaterials();
        SetupTrails();
    }


    private void SetupMaterials()
    {
        materials[0] = new Material(Shader.Find("Shader Graphs/Sh_Ball00"));
        materials[1] = new Material(Shader.Find("Shader Graphs/Sh_Ball00"));

        //Glow Color
        materials[0].SetColor("Color_89166C92", colorPresets[0].colorPresets[1].coreEmissiveColors * 6);
        //Ball Color
        materials[0].SetColor("Color_69EC7551", colorPresets[0].colorPresets[1].coreEmissiveColors);
        //Line Color
        materials[0].SetColor("Color_DE7EE60A", colorPresets[0].colorPresets[1].fresnelColors);

        //Glow Color
        materials[1].SetColor("Color_89166C92", colorPresets[0].colorPresets[2].coreEmissiveColors * 6);
        //Ball Color
        materials[1].SetColor("Color_69EC7551", colorPresets[0].colorPresets[2].coreEmissiveColors);
        //Line Color
        materials[1].SetColor("Color_DE7EE60A", colorPresets[0].colorPresets[2].fresnelColors);


        wallMats[0] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));
        wallMats[1] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));

        wallMats[0].SetColor("_EmissionColor", colorPresets[0].colorPresets[1].coreEmissiveColors);
        wallMats[0].SetFloat("_DissolveDistanceRange", 4f);
        wallMats[0].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        wallMats[0].SetFloat("_AngleRadius", 0.75f);
        wallMats[0].renderQueue = 2800;

        wallMats[1].SetColor("_EmissionColor", colorPresets[0].colorPresets[2].coreEmissiveColors);
        wallMats[1].SetFloat("_DissolveDistanceRange", 4f);
        wallMats[1].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        wallMats[1].SetFloat("_AngleRadius", 0.75f);
        wallMats[1].renderQueue = 2800;



        myRenderer.sharedMaterial = materials[colorID];
        for (int i = 0; i < LevelManager.instance.allMeshes.Length; i++)
        {
            LevelManager.instance.allMeshes[i].sharedMaterial = wallMats[colorID];
        }
    }

    private void SetupTrails()
    {
        trails[0].GetComponent<TrailRenderer>().startColor = colorPresets[0].colorPresets[1].coreEmissiveColors;
        trails[1].GetComponent<TrailRenderer>().startColor = colorPresets[0].colorPresets[2].coreEmissiveColors;

        trails[colorID].SetActive(true);
        trails[((colorID - 1) % trails.Length + trails.Length) % trails.Length].SetActive(false);   // Prevent negative value of modulo
    }



    //Ne plus jamais refaire ça!!!!!!!!!!!
    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
    */
}
