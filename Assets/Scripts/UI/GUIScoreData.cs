using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScoreData : GUIComponent
{
    #region Singleton
    public static GUIScoreData instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Animation")]
    public Animator animScore;

    public override void UpdateText(string newText)
    {
        base.UpdateText(newText);
    }

    // PLAY ANIM FUNCTIONS // 
    public void PlayAnimScoreIncrease()
    {
        animScore.Play("A_Score_Increasing");
    }

}
