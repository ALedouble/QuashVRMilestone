using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Rapport : Score/Scale")]
    public AnimationCurve textValues; //x/Time : scoreValue, y/Value : sizeValue
    public float minTextSize;
    public float maxTextSize;
    public float maxScoreValue;

    [Space]

    [Header("Score variables")]
    public int maxCounter;
    [HideInInspector] public float[] score;
    [HideInInspector] public float[] combo;
    [HideInInspector] public int[] brickCounterGauge;
    

    [HideInInspector] public GUIScoreData[] displayedScore;
    [HideInInspector] public GUIComboData[] displayedCombo;
    [HideInInspector] public bool resetCombo = true;
    [HideInInspector] public PhotonView pV;


    public float finishingFirstScoreBoost;


    public static ScoreManager Instance;

    





    private void Awake()
    {
        pV = GetComponent<PhotonView>();
        Instance = this;
    }


    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
    [PunRPC]
    public void SetScore(int brickValue, int playerID)
    {
        score[playerID] += brickValue * combo[playerID];

        string textScore = score[playerID].ToString();
        displayedScore[playerID].UpdateText(textScore);
    }


    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
    [PunRPC]
    public void SetCombo(int playerID)
    {
        brickCounterGauge[playerID] ++;
        

        if (brickCounterGauge[playerID] >= maxCounter)
        {
            brickCounterGauge[playerID] = 0;
            combo[playerID]++;
            string textCombo = combo[playerID].ToString();
            displayedCombo[playerID].UpdateText("x" + textCombo);

            displayedCombo[playerID].FillImage(brickCounterGauge[playerID] / maxCounter);
        }


        displayedCombo[playerID].FillImage((float)brickCounterGauge[playerID] / (float)maxCounter);
        //Play anim combo increased
    }

    /// <summary>
    /// Reset la valeur du combo
    /// </summary>
    /// <param name="playerID"></param>
    public void ResetCombo(int playerID)
    {
        combo[playerID] = 1;
        brickCounterGauge[playerID] = 0;

        string textCombo = combo[playerID].ToString();
        displayedCombo[playerID].UpdateText("x" + textCombo);
        displayedCombo[playerID].FillImage(brickCounterGauge[playerID] / maxCounter);
    }
}
