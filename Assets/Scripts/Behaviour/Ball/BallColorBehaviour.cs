using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public enum ColorSwitchTrigerType
{
    RACKETBASED,
    WALLBASED
}

public class BallColorBehaviour : MonoBehaviour
{
    public ColorSwitchTrigerType colorSwitchTrigerType = ColorSwitchTrigerType.WALLBASED;

    [Header("Color Settings")]

    // Lier ces materials a un système de color Settings
    public Material[] materials;
    private int colorID = 0;

    private bool isEmpowered;

    private PhotonView photonView;
    private Renderer renderer;

    private void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();

        InitializeSwitchColor();

        photonView = PhotonView.Get(this);

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
            BallEventManager.instance.OnCollisionWithRacket += TransferEmpowerement;
        }
        else if (colorSwitchTrigerType == ColorSwitchTrigerType.RACKETBASED)
        {
            BallEventManager.instance.OnCollisionWithRacket += RacketBaseSwitchColor;
        }
    }

    [PunRPC]
    public void TransferEmpowerement(BallCollisionInfo ballCollisionInfo)
    {
        if(RacketManager.instance.isEmpowered)
        {
            BallBecomeEmpowered();
            //RacketManager.instance.ExitEmpoweredState();
        }
    }

    private void BallBecomeEmpowered()
    {
        isEmpowered = true;
    }

    private void WallBaseSwitchColor(BallCollisionInfo ballCollisionInfo)
    {
        if(isEmpowered)
        {
            isEmpowered = false;
            photonView.RPC("SwitchColor", RpcTarget.All);
        }
    }

    private void RacketBaseSwitchColor(BallCollisionInfo ballCollisionInfo)
    {
        if (RacketManager.instance.isEmpowered)
        {
            if(PhotonNetwork.OfflineMode)
                SwitchColor();
            else
                photonView.RPC("SwitchColor", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SwitchColor()
    {
        //ColorManager.instance.SwitchBallColor();
        colorID = (colorID + 1) % materials.Length;
        renderer.material = materials[colorID];

        if (colorID == 0)
            Debug.Log("Rouge");
        else
            Debug.Log("Bleu");
    }
}
