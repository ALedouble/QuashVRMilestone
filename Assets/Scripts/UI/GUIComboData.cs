using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIComboData : GUIComponent
{
    #region Singleton
    public static GUIComboData instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Animation")]
    public Animator animCombo;

    public override void UpdateText(string newText)
    {
        base.UpdateText(newText);
    }   

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
