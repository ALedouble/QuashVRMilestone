using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLevel : MonoBehaviour
{
    public static MultiLevel Instance;

    [HideInInspector]
    public int levelIndex;


    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }
}
