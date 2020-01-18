using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LayerCompletedEffect : MonoBehaviour
{
    [SerializeField] Animator animator;

    [Button]
    void StartEffect()
    {
        animator.SetTrigger("Start");
    }
}
