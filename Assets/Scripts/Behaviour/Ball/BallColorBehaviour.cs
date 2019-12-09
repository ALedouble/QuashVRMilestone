using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    private Renderer renderer;

    private void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();

        InitializeSwitchColor();

        //Recuperer les couleurs du ColorSettings
    }

    public int GetBallsColor()
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
            SwitchColor();
        }
    }

    private void SwitchColor()
    {
        //ColorManager.instance.SwitchBallColor();
        colorID = (colorID + 1) % colors.Length;
        renderer.material.color = colors[colorID];
    }
}
