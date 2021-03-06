﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public MeshCollider col;

    public void CloseAnim()
    {
        animator.SetTrigger("Start");
    }

    public void OpenAnim()
    {
        animator.SetTrigger("Reset");
    }

}
