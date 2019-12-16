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
    public Color[] colors;
    //public ColorEnum startingColor;
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
        renderer.material.color = colors[colorID];
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
            BallEventManager.instance.OnCollisionWithRacket += SwitchColor;
        }
    }

    [PunRPC]
    public void TransferEmpowerement()
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

    private void WallBaseSwitchColor()
    {
        if(isEmpowered)
        {
            isEmpowered = false;
            SwitchColor();
        }
    }

    private void RacketBaseSwitchColor()
    {
        if (RacketManager.instance.isEmpowered)
        {
            photonView.RPC("SwitchColor", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SwitchColor()
    {
        //ColorManager.instance.SwitchBallColor();
        colorID = (colorID + 1) % colors.Length;
        renderer.material.color = colors[colorID];
    }
}
