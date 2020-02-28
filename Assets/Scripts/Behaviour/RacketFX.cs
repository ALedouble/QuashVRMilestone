using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketFX : MonoBehaviour
{
    public SwitchColorFX[] switchFX = new SwitchColorFX[2];

    private int currentColorID;

    private void Awake()
    {
        currentColorID = -1;
    }

    public void PlaySwitchColorFX()
    {
        currentColorID = RacketManager.instance.RacketColorID - 1;

        for (int i = 0; i < switchFX[currentColorID].burstFX.Length; i++)
        {
            switchFX[currentColorID].burstFX[i].Play();
        }

        for (int i = 0; i < switchFX[currentColorID].loopFX.Length; i++)
        {
            switchFX[currentColorID].loopFX[i].Play();
        }
    }

    public void StopSwitchColorFX()
    {
        if(currentColorID >= 0)
        {
            for (int i = 0; i < switchFX[currentColorID].burstFX.Length; i++)
            {
                switchFX[currentColorID].burstFX[i].Stop();
            }

            for (int i = 0; i < switchFX[currentColorID].loopFX.Length; i++)
            {
                switchFX[currentColorID].loopFX[i].Stop();
            }

            currentColorID = -1;
        }
    }


    public void FXSwitchColorFX()
    {
        if(currentColorID >= 0)
        {
            for (int i = 0; i < switchFX[currentColorID].burstFX.Length; i++)
            {
                switchFX[currentColorID].burstFX[i].Stop();
            }

            for (int i = 0; i < switchFX[currentColorID].loopFX.Length; i++)
            {
                switchFX[currentColorID].loopFX[i].Stop();
            }

            currentColorID = (currentColorID + 1) % 2;

            for (int i = 0; i < switchFX[currentColorID].burstFX.Length; i++)
            {
                switchFX[currentColorID].burstFX[i].Play();
            }

            for (int i = 0; i < switchFX[currentColorID].loopFX.Length; i++)
            {
                switchFX[currentColorID].loopFX[i].Play();
            }
        }
    }

    [System.Serializable]
    public struct SwitchColorFX
    {
        public ParticleSystem[] burstFX;
        public ParticleSystem[] loopFX;
    }
}
