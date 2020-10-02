using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPlatformManager : MonoBehaviour
{
    public static BuildPlatformManager Instance;

    public TargetBuildPlatform targetBuildPlatform;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if(targetBuildPlatform == TargetBuildPlatform.Viveport)
        {
            Viveport.Api.Init(null, "34602bc2-0314-4ddd-8cb2-987150ef458d");
        }
    }

    private void OnApplicationQuit()
    {
        Viveport.Api.Shutdown(null);
    }
}
