using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BallApparitionBehaviour : MonoBehaviour
{
    [Header("References")]
    public SphereCollider col;

    public TextMeshProUGUI secTextMesh;
    public TextMeshProUGUI deciTextMesh;
    public Image loadingCountdownImage;

    public GameObject loadingParent;
    public GameObject blockingParent;
    public GameObject readyParent;

    public Animator animator;
    public AnimationClip succeedAnim;
    bool isApparitionSucceeded = false;

    public void ResumeLoading()
    {
        loadingParent.SetActive(true);
        blockingParent.SetActive(false);
        loadingCountdownImage.gameObject.SetActive(true);

        BallManager.instance.ResumeBall();

        animator.Play("A_ApparitionLoading");

        if (!col.gameObject.activeSelf)
            col.gameObject.SetActive(true);
    }

    public void BlockLoading()
    {
        blockingParent.SetActive(true);
        loadingParent.SetActive(false);

        BallManager.instance.PauseBall();

        animator.Play("A_ApparitionBlocked");
    }

    public void EndLoading()
    {
        if (isApparitionSucceeded)
            return;

        isApparitionSucceeded = true;

        readyParent.SetActive(true);
        loadingParent.SetActive(false);

        if (blockingParent.activeSelf)
            blockingParent.SetActive(false);

        loadingCountdownImage.gameObject.SetActive(false);

        animator.Play("A_ApparitionSucceed");
        col.gameObject.SetActive(false);
    }

    //Update Countdown Display
    public void UpdateLoadingCountdown(float timer, float amount)
    {
        int mySec = (int)timer;
        float myDeci = timer % 1;

        string deci = myDeci.ToString();
        if (deci.Length > 3)
            deci = deci.Remove(0, 2).Remove(2);

        if (secTextMesh.isActiveAndEnabled && deciTextMesh.isActiveAndEnabled)
        {
            secTextMesh.text = mySec.ToString();
            deciTextMesh.text = deci;
        }

        loadingCountdownImage.fillAmount = amount;
    }
}
