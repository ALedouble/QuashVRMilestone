using System.Collections;
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
    //public Material materialBlueprint;
    public PresetScriptable[] colorPresets;
    // Lier ces materials a un système de color Settings
    public Material[] materials;
    public Color testColor;
    public Color testColor2;

    private int colorID = 1;

    private bool isEmpowered;

    private PhotonView photonView;
    private Renderer renderer;

    private void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();

        InitializeSwitchColor();

        photonView = PhotonView.Get(this);

        //materials = new Material[2];
        SetupMaterials();

        //Recuperer les couleurs du ColorSettings
    }

    public int GetBallColor()
    {
        return colorID;
    }

    public void SetBallColor(int colorID)
    {
        this.colorID = colorID;
        renderer.material = materials[colorID];
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
            if(PhotonNetwork.OfflineMode)
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
        colorID = (colorID + 1) % materials.Length;
        renderer.material = materials[colorID];
    }

    private void SetupMaterials()
    {
        //materials[0] = new Material(materialBlueprint);
        //materials[1] = new Material(materialBlueprint);

        materials[0].SetColor("GlowColor", testColor /*colorPresets[0].colorPresets[0].coreEmissiveColors*/);
        materials[0].SetColor("BallColor", testColor /*colorPresets[0].colorPresets[0].fresnelColors*/);

        materials[1].SetColor("GlowColor", testColor2 /*colorPresets[0].colorPresets[1].coreEmissiveColors*/);
        materials[1].SetColor("BallColor", testColor2 /*colorPresets[0].colorPresets[1].fresnelColors*/);

        renderer.material = materials[colorID];
    }

    //Ne plus jamais refaire ça!!!!!!!!!!!
/*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
    */
}
