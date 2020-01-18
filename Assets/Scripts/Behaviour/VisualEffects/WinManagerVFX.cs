using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WinManagerVFX : MonoBehaviour
{
    [SerializeField] WinVFX[] winVFXs;

    [Button]
    public void PlayVFX()
    {
        for (int i = 0; i < winVFXs.Length; i++)
        {
            winVFXs[i].StartWinFX();
        }
    }
}
