using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WinVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particleSystems;
    [SerializeField] float delay = 0.5f;

    [Button]
    public void StartWinFX()
    {
        StartCoroutine(StartVFX());
    }

    IEnumerator StartVFX()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
            yield return new WaitForSeconds(delay);
        }
    }
}
