using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public enum ColorSwitchType
{
    NONE,
    NOSWITCH,
    RACKETEMPOWERED,
    MANDATORY
}

public class BallColorBehaviour : MonoBehaviour//, IPunObservable
{
    [Header("Color Settings")]
    private Material[] materials;

    [Header("Trail Settings")]
    public GameObject[] trails;

    private ColorSwitchType colorSwitchType;

    private int colorID = 1;

    private bool isEmpowered;

    private PhotonView photonView;
    private Renderer myRenderer;


    private void Awake()
    {
        photonView = PhotonView.Get(this);
        myRenderer = gameObject.GetComponent<Renderer>();

        SetupColors();

        colorSwitchType = ColorSwitchType.NONE;
        SetBallColor(LevelManager.instance.currentLevel.level.levelSpec.ballSpawnColorID);
    }

    public void Initialize(ColorSwitchType newColorSwitchType)
    {
        TerminateSwitchColor();

        SetBallColor(LevelManager.instance.currentLevel.level.levelSpec.ballSpawnColorID);

        switch (newColorSwitchType)
        {
            case ColorSwitchType.NONE:
                RacketManager.instance.Initialize(RacketActionType.NONE);
                break;
            case ColorSwitchType.RACKETEMPOWERED:
                BallEventManager.instance.OnCollisionWithRacket += RacketEmpoweredSwitchColor;
                RacketManager.instance.Initialize(RacketActionType.RACKETEMPOWERED);
                break;
            case ColorSwitchType.MANDATORY:
                BallEventManager.instance.OnCollisionWithRacket += MandatorySwitchColor;
                RacketManager.instance.Initialize(RacketActionType.BALLOPPOSITE);
                break;
            default:
                RacketManager.instance.Initialize(RacketActionType.RACKETEMPOWERED);
                break;
        }
    }

    public void TerminateSwitchColor()
    {
        switch (colorSwitchType)
        {
            case ColorSwitchType.RACKETEMPOWERED:
                BallEventManager.instance.OnCollisionWithRacket -= RacketEmpoweredSwitchColor;
                break;
            case ColorSwitchType.MANDATORY:
                BallEventManager.instance.OnCollisionWithRacket -= MandatorySwitchColor;
                break;
            default:
                break;
        }

        colorSwitchType = ColorSwitchType.NONE;
    }

    #region Setup
    public void SetupColors()
    {
        SetupMaterials();
        SetupTrails();
    }

    private void SetupMaterials()
    {
        materials = new Material[3];

        materials[0] = new Material(Shader.Find("Shader Graphs/Sh_Ball00"));
        materials[1] = new Material(Shader.Find("Shader Graphs/Sh_Ball00"));
        materials[2] = new Material(Shader.Find("Shader Graphs/Sh_Ball00"));

        //Glow Color
        materials[0].SetColor("Color_89166C92", LevelManager.instance.colorPresets[0].colorPresets[0].coreEmissiveColors * 6);
        //Ball Color
        materials[0].SetColor("Color_69EC7551", LevelManager.instance.colorPresets[0].colorPresets[0].coreEmissiveColors);
        //Line Color
        materials[0].SetColor("Color_DE7EE60A", LevelManager.instance.colorPresets[0].colorPresets[0].fresnelColors);

        //Glow Color
        materials[1].SetColor("Color_89166C92", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors * 6);
        //Ball Color
        materials[1].SetColor("Color_69EC7551", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors);
        //Line Color
        materials[1].SetColor("Color_DE7EE60A", LevelManager.instance.colorPresets[0].colorPresets[1].fresnelColors);

        //Glow Color
        materials[2].SetColor("Color_89166C92", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors * 6);
        //Ball Color
        materials[2].SetColor("Color_69EC7551", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors);
        //Line Color
        materials[2].SetColor("Color_DE7EE60A", LevelManager.instance.colorPresets[0].colorPresets[2].fresnelColors);


        myRenderer.sharedMaterial = materials[colorID];
    }



    private void SetupTrails()
    {
        trails[0].GetComponent<TrailRenderer>().startColor = LevelManager.instance.colorPresets[0].colorPresets[0].coreEmissiveColors;
        trails[1].GetComponent<TrailRenderer>().startColor = LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors;
        trails[2].GetComponent<TrailRenderer>().startColor = LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors;

        foreach (GameObject trail in trails)
        {
            trail.SetActive(false);
        }
    }
    #endregion

    #region Getter Setter
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

    [PunRPC]
    public void SetBallColor(int colorID)                                                               //A Mettre en reseau!
    {
        this.colorID = colorID;
        myRenderer.sharedMaterial = materials[colorID];
    }
    #endregion

    #region Ball Color Switch
    private void RacketEmpoweredSwitchColor(Collision collision)
    {
        if (RacketManager.instance.IsEmpowered)
            SwitchColor();
    }

    private void MandatorySwitchColor(Collision collision)
    {
        SwitchColor();
    }

    private void SwitchColor()
    {
        if (GameManager.Instance.offlineMode)
        {
            SwitchColorLocally();
        }
        else
        {
            photonView.RPC("SwitchColorLocally", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SwitchColorLocally()
    {
        colorID = (colorID % 2) + 1;
        myRenderer.sharedMaterial = materials[colorID];

        UpdateTrail();

        BallEventManager.instance.SendBallColorSwitchEvent();
    }
    #endregion

    #region Trail Methods
    public void UpdateTrail()
    {
        for (int i = 0; i < trails.Length; i++)
        {
            trails[i].SetActive(i == colorID);
        }
    }

    public void DeactivateTrail()
    {
        foreach (GameObject trail in trails)
        {
            trail.SetActive(false);
        }
    }
    #endregion

    #region Ball First Spawn Coroutine
    public void StartBallFirstSpawnCoroutine(float duration)
    {
        StartCoroutine(BallFirstSpawnCoroutine(duration));
    }

    private IEnumerator BallFirstSpawnCoroutine(float duration)
    {
        yield return new WaitForEndOfFrame();

        float initialGlowPower = GetCurrentMaterial().GetFloat("Vector1_5584EFD3");
        float initalAlpha = GetCurrentMaterial().GetFloat("Vector1_D0CFE999");

        float timeElapsed = 0f;
        bool isBlockPenalityApplied = false;
        while (timeElapsed < duration)
        {
            float timeRate = timeElapsed / duration;

            GetCurrentMaterial().SetFloat("Vector1_5584EFD3", initialGlowPower * timeRate);
            GetCurrentMaterial().SetFloat("Vector1_D0CFE999", 0);

            if (BallManager.instance.BallApparitionBehaviour != null)
                BallManager.instance.BallApparitionBehaviour.UpdateLoadingCountdown((duration - timeElapsed), timeRate);

            //if(duration - timeElapsed <= BallManager.instance.BallApparitionBehaviour.succeedAnim.length)                 /// if the "succeed animation" is essential, we can make it appeared before time ends

            yield return new WaitForFixedUpdate();
            if (!BallManager.instance.IsBallPaused)
            {
                timeElapsed += Time.fixedDeltaTime;
                isBlockPenalityApplied = false;
            }
            else if (!isBlockPenalityApplied)
            {
                timeElapsed -= Time.fixedDeltaTime;
                isBlockPenalityApplied = true;
            }
        }

        GetCurrentMaterial().SetFloat("Vector1_5584EFD3", initialGlowPower);
        GetCurrentMaterial().SetFloat("Vector1_D0CFE999", initalAlpha);
        BallManager.instance.BallApparitionBehaviour.EndLoading();
    }
    #endregion
}