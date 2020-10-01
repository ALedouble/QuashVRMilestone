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
    public Color32 completedScoreColor;

    [HideInInspector] public bool isThereScoreCondition;
    [HideInInspector] public bool isThereComboCondition;
    [HideInInspector] public int scoreConditionValue;
    [HideInInspector] public int comboConditionValue;
    private bool hasScoreConditionSucceeded = false;
    private bool hasComboConditionSucceeded = false;

    [Space]

    [Header("Score variables")]
    public int maxCounter;
    [HideInInspector] public float[] score;
    [HideInInspector] public int[] combo;
    [HideInInspector] public int[] brickCounterGauge;
    public int[] playersMaxCombo;


    [HideInInspector] public GUIScoreData[] displayedScore;
    [HideInInspector] public GUIComboData[] displayedCombo;
    [HideInInspector] public bool resetCombo = true;

    [Header("Exotic variables")]
    public float bonusScoreOnTimeAttack = 0;

    private Coroutine remoteUpdateCoroutine;


    //Delegate
    public delegate void ScoreManagerDelegate();
    public event ScoreManagerDelegate OnComboReset;

    private PhotonView photonView;

    public static ScoreManager Instance;

    private void Awake()
    {
        Instance = this;

        photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
    [PunRPC]
    public void SetScore(int brickValue, int playerID)
    {
        if (playerID == (int)QPlayerManager.instance.LocalPlayerID)
        {
            score[playerID] += brickValue * combo[playerID];

            if (!hasScoreConditionSucceeded && isThereScoreCondition)
            {
                //Debug.Log("Condition Check");
                if (score[playerID] >= scoreConditionValue)
                {
                    //Debug.Log("Score Anim PLEASE");
                    LevelManager.instance.playersHUD.ScoreConditionCompleted();
                    hasScoreConditionSucceeded = true;
                    StartCoroutine(ConditionCompleteTime());
                }
            }

            string textScore = score[playerID].ToString();
            displayedScore[playerID].UpdateText(textScore);

            if(!GameManager.Instance.offlineMode && remoteUpdateCoroutine == null)
            {
                remoteUpdateCoroutine = StartCoroutine(RemoteUpdateCoroutine(playerID));
            }
        }
    }

    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
    [PunRPC]
    public void SetCombo(int playerID)
    {
        brickCounterGauge[playerID]++;

        if (brickCounterGauge[playerID] >= maxCounter)
        {
            brickCounterGauge[playerID] = 0;
            combo[playerID]++;
            string textCombo = combo[playerID].ToString();
            displayedCombo[playerID].UpdateText("x" + textCombo);
            GUIComboAnim.instance.PlayAnimComboIncreasing();

            displayedCombo[playerID].FillImage(brickCounterGauge[playerID] / maxCounter);

            if (combo[playerID] > playersMaxCombo[playerID])
            {
                playersMaxCombo[playerID] = combo[playerID];
            }
        }

        displayedCombo[playerID].FillImage((float)brickCounterGauge[playerID] / (float)maxCounter);

        if (!GameManager.Instance.offlineMode && playerID == (int)QPlayerManager.instance.LocalPlayerID && remoteUpdateCoroutine == null)
        {
            remoteUpdateCoroutine = StartCoroutine(RemoteUpdateCoroutine(playerID));
        }
    }

    /// <summary>
    /// Reset la valeur du combo
    /// </summary>
    /// <param name="playerID"></param>
    [PunRPC]
    public void ResetCombo(int playerID)
    {
        combo[playerID] = 1;
        brickCounterGauge[playerID] = 0;

        string textCombo = combo[playerID].ToString();
        displayedCombo[playerID].UpdateText("x" + textCombo);
        displayedCombo[playerID].FillImage(brickCounterGauge[playerID] / maxCounter);

        //Trigger de l'anim
        GUIComboAnim.instance.PlayAnimComboBreak();

        //Trigger de la vibration
        //VibrationManager.instance.VibrateOn("Vibration_Mistake");

        //Trigger du son
        AudioManager.instance.PlaySound("Break_Combo", Vector3.zero);

        OnComboReset(); //Combo reset delegate
    }

    /// <summary>
    /// Time Attack Behaviour
    /// </summary>
    public void OnTimeAttackBonus()
    {
        int bonus = (int)(TimeManager.Instance.CurrentTimer * bonusScoreOnTimeAttack);
        SetScore(bonus, 0);
        //Debug.Log("Score bonus of " + bonus);
    }

    public void CheckForComboBreak()
    {
        resetCombo = true;
        StartCoroutine(CheckComboCondition(ExplosionManager.Instance.impactDuration, (int)QPlayerManager.instance.LocalPlayerID));
    }

    private IEnumerator CheckComboCondition(float timeBeforeCheck, int playerID)
    {
        yield return new WaitForSeconds(timeBeforeCheck);

        if (resetCombo)
        {
            if(GameManager.Instance.offlineMode)
            {
                ResetCombo(playerID);
            }
            else
            {
                photonView.RPC("ResetCombo", RpcTarget.All, playerID);
            }
        }
    }

    public void BuildScoreText(int scoreValue, int colorID, Vector3 position, Quaternion rotation)
    {
        //GameObject scoreText = PoolManager.instance.SpawnFromPool("ScoreText", position, rotation);
        //float newScore = scoreValue * combo[(int)BallManager.instance.GetLastPlayerWhoHitTheBall()];
        //scoreText.GetComponent<HitScoreBehaviour>().SetHitValues(newScore, LevelManager.instance.colorPresets[0].colorPresets[colorID].coreEmissiveColors);

        PoolManager.instance.SpawnFromPool("ScoreText", position, rotation).GetComponent<HitScoreBehaviour>().SetHitValues(scoreValue * combo[(int)BallManager.instance.GetLastPlayerWhoHitTheBall()], LevelManager.instance.colorPresets[0].colorPresets[colorID].coreEmissiveColors);
    }

    IEnumerator ConditionCompleteTime()
    {
        displayedScore[0].cannotPlayAnim = true;

        yield return new WaitForSeconds(0.15f);

        displayedScore[0].UpdateTextColor(completedScoreColor);

        yield return new WaitForSeconds(0.30f);

        displayedScore[0].cannotPlayAnim = false;
    }


    #region Multiplayer

    private IEnumerator RemoteUpdateCoroutine(int playerID)
    {
        yield return new WaitForEndOfFrame();

        photonView.RPC("RemoteUpdate", RpcTarget.Others, playerID, score[playerID], combo[playerID]);

        remoteUpdateCoroutine = null;
    }

    [PunRPC]
    private void RemoteUpdate(int playerID, float newScore, int newCombo)
    {
        score[playerID] = newScore;

        if (!hasScoreConditionSucceeded && isThereScoreCondition)
        {
            //Debug.Log("Condition Check");
            if (score[playerID] >= scoreConditionValue)
            {
                //Debug.Log("Score Anim PLEASE");
                LevelManager.instance.playersHUD.ScoreConditionCompleted();
                hasScoreConditionSucceeded = true;
                StartCoroutine(ConditionCompleteTime());
            }
        }

        string textScore = score[playerID].ToString();
        displayedScore[playerID].UpdateText(textScore);

        if( newCombo > combo[playerID])
        {
            while (combo[playerID] < newCombo)
                SetCombo(playerID);
        }
    }

    public void SendResetComboRPC(int playerID)
    {
        photonView.RPC("ResetCombo", RpcTarget.All, playerID);
    }
    
    #endregion

}
