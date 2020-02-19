using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketFX : MonoBehaviour
{
    public SwitchColorFX[] switchFX = new SwitchColorFX[2];

    public void PlaySwitchColorFX(int colorID)
    {
        for (int i = 0; i < switchFX[colorID].burstFX.Length; i++)
        {
            switchFX[colorID].burstFX[i].Play();
        }

        for (int i = 0; i < switchFX[colorID].loopFX.Length; i++)
        {
            switchFX[colorID].loopFX[i].Play();
        }
    }

    public void StopSwitchColorFX(int colorID)
    {
        for (int i = 0; i < switchFX[colorID].burstFX.Length; i++)
        {
            switchFX[colorID].burstFX[i].Play();
        }

        for (int i = 0; i < switchFX[colorID].loopFX.Length; i++)
        {
            switchFX[colorID].loopFX[i].Play();
        }
    }

    [System.Serializable]
    public struct SwitchColorFX
    {
        public ParticleSystem[] burstFX;
        public ParticleSystem[] loopFX;
    }
}
