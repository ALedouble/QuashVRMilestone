using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public enum ColorSwitchBehaviour
{
    NORMAL,
    MANDATORY
}

public class BallColorBehaviour : MonoBehaviour//, IPunObservable
{
    [Header("Color Settings")]
    private Material[] materials;

    ///////////////////////////////
    private Material[] sideWallMats;                                        // Ca devrait pas etre la!
    private Material[] midWallMats;                                         // Ca devrait pas etre la!
    ///////////////////////////////

    [Header("Trail Settings")]
    public GameObject[] trails;


    private int colorID = 1;

    private bool isEmpowered;

    private PhotonView photonView;
    private Renderer myRenderer;



    private void Awake()
    {
        photonView = PhotonView.Get(this);
        myRenderer = gameObject.GetComponent<Renderer>();

        SetupColors();
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

    public void SetBallColor(int colorID)                                                               //A Mettre en reseau!
    {
        this.colorID = colorID;
        myRenderer.material = materials[colorID];
    }

    private void InitializeSwitchColor()
    {
        BallEventManager.instance.OnCollisionWithRacket += RacketBaseSwitchColor;
    }

    [PunRPC]
    public void TransferEmpowerement()
    {
        if (RacketManager.instance.isEmpowered)
        {
            if (GameManager.Instance.offlineMode)
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

    private void RacketBaseSwitchColor()
    {
        if (RacketManager.instance.isEmpowered)
        {
            if (GameManager.Instance.offlineMode)
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
        myRenderer.sharedMaterial = materials[colorID];

        UpdateTrail();

        SwitchWallColors();

        if (RacketManager.instance.isEmpowered)
        {
            RacketManager.instance.SwitchRacketColor();
            RacketManager.instance.localRacketFX.FXSwitchColorFX();

            if (!GameManager.Instance.offlineMode)
                RacketManager.instance.foreignRacketFX.FXSwitchColorFX();
        }
    }


    private void SwitchWallColors()
    {
        for (int i = 0; i < LevelManager.instance.allMeshes.Length; i++)
        {
            LevelManager.instance.allMeshes[i].sharedMaterial = sideWallMats[colorID];
        }

        if (LevelManager.instance.numberOfPlayers > 1)
        {
            LevelManager.instance.midMesh.sharedMaterial = midWallMats[colorID];
        }
    }
    public void SetupColors()
    {
        //if (GameManager.Instance.offlineMode)                                                           // Besoin de mise en reseau?
        //{
            SetupColorsLocaly();
        //}
        //else if (PhotonNetwork.IsMasterClient)
        //{
        //    photonView.RPC("SetupColors", RpcTarget.AllBuffered);
        //}
    }
    
    [PunRPC]
    private void SetupColorsLocaly()
    {
        SetupMaterials();
        /////////////////////
        SetupWallMaterials();                                       // Ca devrait pas être la!
        /////////////////////
        SetupTrails();
    }


    private void SetupMaterials()
    {
        materials = new Material[2];

        materials[0] = new Material(Shader.Find("Shader Graphs/Sh_Ball00"));
        materials[1] = new Material(Shader.Find("Shader Graphs/Sh_Ball00"));

        //Glow Color
        materials[0].SetColor("Color_89166C92", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors * 6);
        //Ball Color
        materials[0].SetColor("Color_69EC7551", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors);
        //Line Color
        materials[0].SetColor("Color_DE7EE60A", LevelManager.instance.colorPresets[0].colorPresets[1].fresnelColors);

        //Glow Color
        materials[1].SetColor("Color_89166C92", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors * 6);
        //Ball Color
        materials[1].SetColor("Color_69EC7551", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors);
        //Line Color
        materials[1].SetColor("Color_DE7EE60A", LevelManager.instance.colorPresets[0].colorPresets[2].fresnelColors);


        myRenderer.sharedMaterial = materials[colorID];
        
    }

    public void SetupWallMaterials()                                                                                                        // Ca devrait pas être la!
    {
        sideWallMats = new Material[2];
        midWallMats = new Material[2];

        sideWallMats[0] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));
        sideWallMats[1] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));

        sideWallMats[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors);
        sideWallMats[0].SetFloat("_DissolveDistanceRange", 4f);
        sideWallMats[0].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        sideWallMats[0].SetFloat("_AngleRadius", 0.75f);
        sideWallMats[1].SetFloat("_GridAlpha", 0.5f);
        sideWallMats[0].renderQueue = 2800;

        sideWallMats[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors);
        sideWallMats[1].SetFloat("_DissolveDistanceRange", 4f);
        sideWallMats[1].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        sideWallMats[1].SetFloat("_AngleRadius", 0.75f);
        sideWallMats[1].SetFloat("_GridAlpha", 0.5f);
        sideWallMats[1].renderQueue = 2800;


        midWallMats[0] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));
        midWallMats[1] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));

        midWallMats[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors);
        midWallMats[0].SetFloat("_DissolveDistanceRange", 4f);
        midWallMats[0].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        midWallMats[0].SetFloat("_AngleRadius", 0.75f);
        midWallMats[0].renderQueue = 3000;

        midWallMats[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors);
        midWallMats[1].SetFloat("_DissolveDistanceRange", 4f);
        midWallMats[1].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        midWallMats[1].SetFloat("_AngleRadius", 0.75f);
        midWallMats[1].renderQueue = 3000;

        for (int i = 0; i < LevelManager.instance.allMeshes.Length; i++)
        {
            LevelManager.instance.allMeshes[i].sharedMaterial = sideWallMats[colorID];
        }

        if (LevelManager.instance.numberOfPlayers > 1)
        {
            LevelManager.instance.midMesh.sharedMaterial = midWallMats[colorID];
        }
    }

    private void SetupTrails()
    {
        trails[0].GetComponent<TrailRenderer>().startColor = LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors;
        trails[1].GetComponent<TrailRenderer>().startColor = LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors;

        trails[colorID].SetActive(true);
        trails[((colorID - 1) % trails.Length + trails.Length) % trails.Length].SetActive(false);   // Prevent negative value of modulo
    }

    public void UpdateTrail()
    {
        trails[colorID].SetActive(true);
        trails[((colorID - 1) % trails.Length + trails.Length) % trails.Length].SetActive(false);
    }

    public void DeactivateTrail()
    {
        trails[colorID].SetActive(false);
    }

    public void StartBallFirstSpawnCoroutine(float duration)
    {
        StartCoroutine(BallFirstSpawnCoroutine(duration));
    }

    private IEnumerator BallFirstSpawnCoroutine(float duration)
    {
        yield return new WaitForEndOfFrame();

        float initialGlowPower = GetCurrentMaterial().GetFloat("Vector1_5584EFD3");
        float timeElapsed = 0f;
        
        while(timeElapsed < duration)
        {
            GetCurrentMaterial().SetFloat("Vector1_5584EFD3", initialGlowPower * timeElapsed / duration);
            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }
        
        GetCurrentMaterial().SetFloat("Vector1_5584EFD3", initialGlowPower);
    }
}