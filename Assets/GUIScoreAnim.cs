using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScoreAnim : MonoBehaviour
{
    #region Singleton
    public static GUIScoreAnim instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Animation")]
    public Animator animScore;

    // PLAY ANIM FUNCTIONS // 
    public void PlayAnimScoreIncrease()
    {
        animScore.Play("A_Score_Increasing");
    }
}
