using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIComboAnim : MonoBehaviour
{
    #region Singleton
    public static GUIComboAnim instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Animation")]
    public Animator animCombo;

    // PLAY ANIM FUNCTIONS // 
    public void PlayAnimComboIncreasing()
    {
        animCombo.Play("A_Combo_Increasing");
    }

    public void PlayAnimComboBreak()
    {
        animCombo.Play("A_Combo_Break");
    }
}
