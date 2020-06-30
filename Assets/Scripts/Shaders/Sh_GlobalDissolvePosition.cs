using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sh_GlobalDissolvePosition : MonoBehaviour
{
    public static Transform ballTransform;
    public static int WallColorID { get; set; }

    private static Material[] sideWallMats;                                       
    private static Material[] midWallMats;  

    private void Update()
    {
        if (ballTransform)
            Shader.SetGlobalVector("_MagicalBallPos", ballTransform.position);
    }

    public static void Setup()
    {
        if (BallManager.instance)
        {
            ballTransform = BallManager.instance.Ball.transform;
            WallColorID = BallManager.instance.BallColorBehaviour.GetBallColor();
            SetupWallMaterials();

            BallEventManager.instance.OnBallColorSwitch += SwitchWallColors;
        }
    }

    #region SetupMethods

    private static void SetupWallMaterials()                                                                                                        // Ca devrait pas être la!
    {
        sideWallMats = new Material[3];
        midWallMats = new Material[3];

        sideWallMats[0] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));
        sideWallMats[1] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));
        sideWallMats[2] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));

        sideWallMats[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[0].coreEmissiveColors);
        sideWallMats[0].SetFloat("_DissolveDistanceRange", 4f);
        sideWallMats[0].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        sideWallMats[0].SetFloat("_AngleRadius", 0.75f);
        sideWallMats[0].SetFloat("_GridAlpha", 0.5f);
        sideWallMats[0].renderQueue = 2800;

        sideWallMats[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors);
        sideWallMats[1].SetFloat("_DissolveDistanceRange", 4f);
        sideWallMats[1].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        sideWallMats[1].SetFloat("_AngleRadius", 0.75f);
        sideWallMats[1].SetFloat("_GridAlpha", 0.5f);
        sideWallMats[1].renderQueue = 2800;

        sideWallMats[2].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors);
        sideWallMats[2].SetFloat("_DissolveDistanceRange", 4f);
        sideWallMats[2].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        sideWallMats[2].SetFloat("_AngleRadius", 0.75f);
        sideWallMats[2].SetFloat("_GridAlpha", 0.5f);
        sideWallMats[2].renderQueue = 2800;


        midWallMats[0] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));
        midWallMats[1] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));
        midWallMats[2] = new Material(Shader.Find("Shader Graphs/Sh_SideWalls02"));

        midWallMats[0].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[0].coreEmissiveColors);
        midWallMats[0].SetFloat("_DissolveDistanceRange", 4f);
        midWallMats[0].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        midWallMats[0].SetFloat("_AngleRadius", 0.75f);
        midWallMats[0].renderQueue = 3000;
        
        midWallMats[1].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[1].coreEmissiveColors);
        midWallMats[1].SetFloat("_DissolveDistanceRange", 4f);
        midWallMats[1].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        midWallMats[1].SetFloat("_AngleRadius", 0.75f);
        midWallMats[1].renderQueue = 3000;

        midWallMats[2].SetColor("_EmissionColor", LevelManager.instance.colorPresets[0].colorPresets[2].coreEmissiveColors);
        midWallMats[2].SetFloat("_DissolveDistanceRange", 4f);
        midWallMats[2].SetVector("_Tiling", new Vector4(3, 3, 0, 0));
        midWallMats[2].SetFloat("_AngleRadius", 0.75f);
        midWallMats[2].renderQueue = 3000;

        for (int i = 0; i < LevelManager.instance.allMeshes.Length; i++)
        {
            LevelManager.instance.allMeshes[i].sharedMaterial = sideWallMats[WallColorID];
        }

        if (LevelManager.instance.numberOfPlayers > 1)
        {
            LevelManager.instance.midMesh.sharedMaterial = midWallMats[WallColorID];
        }
    }

    #endregion

    public static void SwitchWallColors()
    {
        WallColorID = BallManager.instance.BallColorBehaviour.GetBallColor();

        for (int i = 0; i < LevelManager.instance.allMeshes.Length; i++)
        {
            LevelManager.instance.allMeshes[i].sharedMaterial = sideWallMats[WallColorID];
        }

        if (LevelManager.instance.numberOfPlayers > 1)
        {
            LevelManager.instance.midMesh.sharedMaterial = midWallMats[WallColorID];
        }
    }
}
