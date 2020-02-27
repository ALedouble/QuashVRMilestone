using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketFX : MonoBehaviour
{
    public SwitchColorFX[] switchFX = new SwitchColorFX[2];

    public void PlaySwitchColorFX()
    {
        int colorID = (BallManager.instance.GetBallColorID() + 1) % 2;

        for (int i = 0; i < switchFX[colorID].burstFX.Length; i++)
        {
            switchFX[colorID].burstFX[i].Play();
        }

        for (int i = 0; i < switchFX[colorID].loopFX.Length; i++)
        {
            switchFX[colorID].loopFX[i].Play();
        }
    }

    public void StopSwitchColorFX()
    {
        int colorID = (BallManager.instance.GetBallColorID() + 1) % 2;

        for (int i = 0; i < switchFX[colorID].burstFX.Length; i++)
        {
            switchFX[colorID].burstFX[i].Stop();
        }

        for (int i = 0; i < switchFX[colorID].loopFX.Length; i++)
        {
            switchFX[colorID].loopFX[i].Stop();
        }
    }


    public void FXSwitchColorFX()
    {
        int oldColorID = (BallManager.instance.GetBallColorID());
        int newColorID = (BallManager.instance.GetBallColorID() + 1) % 2;

        for (int i = 0; i < switchFX[newColorID].burstFX.Length; i++)
        {
            switchFX[newColorID].burstFX[i].Play();
        }

        for (int i = 0; i < switchFX[newColorID].loopFX.Length; i++)
        {
            switchFX[newColorID].loopFX[i].Play();
        }

        for (int i = 0; i < switchFX[oldColorID].burstFX.Length; i++)
        {
            switchFX[oldColorID].burstFX[i].Stop();
        }

        for (int i = 0; i < switchFX[oldColorID].loopFX.Length; i++)
        {
            switchFX[oldColorID].loopFX[i].Stop();
        }
    }

    [System.Serializable]
    public struct SwitchColorFX
    {
        public ParticleSystem[] burstFX;
        public ParticleSystem[] loopFX;
    }
}
