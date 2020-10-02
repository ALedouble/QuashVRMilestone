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
    }
}
