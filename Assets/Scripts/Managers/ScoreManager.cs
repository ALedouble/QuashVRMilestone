using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Rapport : Score/Scale")]
    public AnimationCurve textValues; //x/Time : scoreValue, y/Value : sizeValue
    public float minTextSize;
    public float maxTextSize;
    public float maxScoreValue;

    [Header("Score")]
    public float[] score;
    public float[] combo;
    public int[] brickCounterGauge;
    public int maxCounter;

    public GUIScoreData[] displayedScore;
    public GUIComboData[] displayedCombo;
    public bool resetCombo = true;


    public float finishingFirstScoreBoost;


    public static ScoreManager Instance;





    private void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
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
