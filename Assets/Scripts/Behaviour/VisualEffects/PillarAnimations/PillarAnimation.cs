using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void StartOpenAnim()
    {
        animator.SetTrigger("Start");
    }

    public void StartCloseAnim()
    {
        animator.SetTrigger("Reset");
    }

}
