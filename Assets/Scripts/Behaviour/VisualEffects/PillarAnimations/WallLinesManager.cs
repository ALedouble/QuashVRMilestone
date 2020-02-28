using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WallLinesManager : MonoBehaviour
{
    [SerializeField] PillarAnimation[] pillarAnimations;
    [SerializeField] float delay = 0.25f;

    [Button]
    public void OpenLines()
    {
        StartCoroutine(OpenAllLines());
    }

    [Button]
    public void CloseLines()
    {
        StartCoroutine(CloseAllLines());
    }

    IEnumerator OpenAllLines()
    {
        for (int i = 0; i < pillarAnimations.Length; i++)
        {
            pillarAnimations[i].StartOpenAnim();
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator CloseAllLines()
    {
        for (int i = 0; i < pillarAnimations.Length; i++)
        {
            pillarAnimations[i].StartCloseAnim();
            yield return new WaitForSeconds(delay);
        }
    }
}
