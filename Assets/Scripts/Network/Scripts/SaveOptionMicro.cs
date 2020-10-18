using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveOptionMicro : MonoBehaviour
{
    public static SaveOptionMicro Instance;

    public bool enabledComms = true;
    public bool isMuted;
    public float volumeValue = 1f;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
