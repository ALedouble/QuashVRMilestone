using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LayerCompletedEffect : MonoBehaviour
{
    [SerializeField] Animator animator;

    [Button]
    public void StartEffect()
    {
        animator.SetTrigger("Start");
    }
}
