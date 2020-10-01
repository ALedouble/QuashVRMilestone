using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIHUD : MonoBehaviour
{
    [Header("SOLO ONLY")]
    [SerializeField] GUIScoreConditionData scoreConditionData;
    //[SerializeField] GUITimerConditionData comboConditionData;
    [SerializeField] GUITimerConditionData timerConditionData;
    [SerializeField] GameObject[] scoreConditionParents;
    [SerializeField] GameObject[] comboConditionParents;
    [SerializeField] GameObject[] timerConditionParents;
    [SerializeField] Animator timerFailedAnim;
    [SerializeField] Animator scoreCompletedAnim;
    [SerializeField] GameObject[] completedStars;

    public GameObject[] TimerStars;

    [Header("HUD References")]
    [SerializeField] GUIScoreData[] scoreDATAs;
    [SerializeField] GUIComboData[] comboData;
    [SerializeField] GUIFillBarData fillBarData;
    [SerializeField] GUITimerData[] timerData;

    [Header("HUD ScoreScreen References")]
    [SerializeField] GUIMaxScoreData[] maxScoreData;
    [SerializeField] GUIMaxComboData[] maxComboData;

    [Header("References")]
    [SerializeField] Animator[] animScoreScreen;
    [SerializeField] GameObject[] scoreScreenCompleted;
    [SerializeField] GameObject[] scoreScreenFailed;
    [SerializeField] GameObject[] scoreComboHUD;
    [SerializeField] GameObject timerHUD;
    [SerializeField] GameObject[] layerCountHUD;
    [SerializeField] GameObject[] scoreScreenHUD;

    [SerializeField] VRTK.VRTK_UICanvas[] scoreScreenUICanvas;

    public Transform[] layerCountParent;

    #region Singleton
    public static GUIHUD instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    public GUIScoreData[] ScoreDATAs { get => scoreDATAs; }
    public GUIScoreConditionData ScoreConditionData { get => scoreConditionData; }
    //public GUIScoreConditionData ComboConditionData { get => comboConditionData; }
    public GUITimerConditionData TimerConditionData { get => timerConditionData; }
    public GameObject[] ScoreConditionParents { get => scoreConditionParents; }
    //public GameObject[] ComboConditionParents { get => comboConditionParents; }
    public GameObject[] TimerConditionParents { get => timerConditionParents; }

    

    public GUIComboData[] ComboData { get => comboData; }
    public GUIFillBarData FillBarData { get => fillBarData; }
    public GUITimerData[] TimerData { get => timerData; }
    public GUIMaxScoreData[] MaxScoreData { get => maxScoreData; }
    public GUIMaxComboData[] MaxComboData { get => maxComboData; }

    // ----------------------------------------------------- //

    public void EnableScoreScreen()
    {
        for (int i = 0; i < animScoreScreen.Length; i++)
        {
            scoreScreenHUD[i].SetActive(true);

            StartCoroutine(TimerUI(i));

            animScoreScreen[i].Play("A_ScoreScreen_Appearing");

            scoreComboHUD[i].SetActive(false);
            layerCountHUD[i].SetActive(false);

            maxComboData[i].UpdateText(ScoreManager.Instance.playersMaxCombo[i].ToString());
            maxScoreData[i].UpdateText(ScoreManager.Instance.score[i].ToString());

            int length = LevelManager.instance.currentLevel.level.levelProgression.numberOfAdditionalConditions;

            if (length > 0 && GameManager.Instance.offlineMode)
            {
                for (int y = 0; y < length; y++)
                {
                    completedStars[y + 1].transform.parent.gameObject.SetActive(true);
                }
            }
        }

        timerHUD.SetActive(false);

        if (!GameManager.Instance.HasLost)
        {
            // ------------------------- VICTORY ------------------------- //
            if (scoreScreenFailed.Length > 1)
            {
                scoreScreenFailed[1].SetActive(true);
            }

            scoreScreenCompleted[0].SetActive(true);



            ///////////////////////////// Checking Condition 4 STARS /////////////////////////////////////
            if (GameManager.Instance.offlineMode)
            {
                completedStars[0].transform.parent.gameObject.SetActive(true);
                completedStars[0].SetActive(true);

                int length = LevelManager.instance.currentLevel.level.levelProgression.numberOfAdditionalConditions;
                LevelsScriptable level = LevelManager.instance.currentLevel;

                int score = (int)ScoreManager.Instance.score[0];
                int combo = ScoreManager.Instance.playersMaxCombo[0];
                int time = (int)TimeManager.Instance.CurrentTimer;

                SavedValues levelValue = new SavedValues
                {
                    unlock = true,
                    done = true,
                    bestCombo = level.level.levelProgression.maxCombo,
                    bestScore = level.level.levelProgression.maxScore,
                    bestTime = level.level.levelProgression.minTiming
                };

                bool isThereComboCondition = false;
                bool isThereScoreCondition = false;
                bool isThereTimeCondition = false;

                if (length > 0)
                {


                    //Debug.Log("score : " + score);
                    //Debug.Log("combo : " + combo);
                    //Debug.Log("time : " + time);

                    for (int i = 0; i < level.level.levelProgression.numberOfAdditionalConditions; i++)
                    {
                        if (level.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
                        {
                            switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                            {
                                case CompleteConditionType.Score:
                                    //Debug.Log("Checking Score");
                                    if (score > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                    {
                                        if (score > level.level.levelProgression.maxScore)
                                            levelValue.bestScore = score;
                                        else
                                            levelValue.bestScore = level.level.levelProgression.maxScore;

                                        completedStars[i + 1].SetActive(true);
                                    }
                                    else
                                        levelValue.bestScore = level.level.levelProgression.maxScore;

                                    isThereScoreCondition = true;
                                    break;

                                case CompleteConditionType.Combo:
                                    if (combo > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                    {
                                        if (combo > level.level.levelProgression.maxCombo)
                                            levelValue.bestCombo = combo;
                                        else
                                            levelValue.bestCombo = level.level.levelProgression.maxCombo;

                                        completedStars[i + 1].SetActive(true);
                                    }
                                    else
                                        levelValue.bestCombo = level.level.levelProgression.maxCombo;

                                    isThereComboCondition = true;
                                    break;

                                case CompleteConditionType.Timing:
                                    if (time > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                    {
                                        if (time > level.level.levelProgression.minTiming)
                                        {
                                            levelValue.bestTime = time;
                                        }
                                        else
                                        {
                                            levelValue.bestTime = level.level.levelProgression.minTiming;
                                        }

                                        completedStars[i + 1].SetActive(true);
                                    }
                                    else
                                        levelValue.bestTime = level.level.levelProgression.minTiming;

                                    isThereTimeCondition = true;
                                    break;
                            }
                        }
                        else
                        {
                            switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                            {
                                case CompleteConditionType.Score:
                                    if (score < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                    {
                                        if (score > level.level.levelProgression.maxScore)
                                            levelValue.bestScore = score;
                                        else
                                            levelValue.bestScore = level.level.levelProgression.maxScore;

                                        completedStars[i + 1].SetActive(true);
                                    }
                                    else
                                        levelValue.bestScore = level.level.levelProgression.maxScore;

                                    isThereScoreCondition = true;
                                    break;

                                case CompleteConditionType.Combo:
                                    if (combo < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                    {
                                        if (combo > level.level.levelProgression.maxCombo)
                                            levelValue.bestCombo = combo;
                                        else
                                            levelValue.bestCombo = level.level.levelProgression.maxCombo;

                                        completedStars[i + 1].SetActive(true);
                                    }
                                    else
                                        levelValue.bestCombo = level.level.levelProgression.maxCombo;

                                    isThereComboCondition = true;
                                    break;

                                case CompleteConditionType.Timing:
                                    if (time < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                    {
                                        if (time < level.level.levelProgression.minTiming)
                                            levelValue.bestTime = time;
                                        else
                                            levelValue.bestTime = level.level.levelProgression.minTiming;

                                        completedStars[i + 1].SetActive(true);
                                    }
                                    else
                                    {
                                        if (time < level.level.levelProgression.minTiming)
                                            levelValue.bestTime = time;
                                        else
                                            levelValue.bestTime = level.level.levelProgression.minTiming;
                                    }
                                        

                                    isThereTimeCondition = true;
                                    break;
                            }
                        }
                    }
                    //Debug.Log("isThereComboCondition : " + isThereComboCondition);
                    //Debug.Log("isThereScoreCondition : " + isThereScoreCondition);
                    //Debug.Log("isThereTimeCondition : " + isThereTimeCondition);
                }

                if (!isThereComboCondition)
                {
                    //Debug.Log("NO COMBO CONDITION");

                    if (combo > level.level.levelProgression.maxCombo)
                        levelValue.bestCombo = combo;
                    else
                        levelValue.bestCombo = level.level.levelProgression.maxCombo;
                }

                if (!isThereTimeCondition)
                {
                    //Debug.Log("NO SCORE CONDITION");

                    if (time > level.level.levelProgression.minTiming)
                        levelValue.bestTime = time;
                    else
                        levelValue.bestTime = level.level.levelProgression.minTiming;
                }

                if (!isThereScoreCondition)
                {
                    //Debug.Log("NO TIME CONDITION");

                    if (score > level.level.levelProgression.maxScore)
                        levelValue.bestScore = score;
                    else
                        levelValue.bestScore = level.level.levelProgression.maxScore;
                }


                JSON.instance.SaveLevelDATA(levelValue, level.level.levelProgression.LevelIndex);
            }

            // ------- PLAY SOUND ------- //
            AudioManager.instance.PlaySound("Victory", Vector3.zero);

        }
        else
        {
            // ------------------------- DEFEAT ------------------------- //
            if (scoreScreenCompleted.Length > 1)
            {
                scoreScreenCompleted[1].SetActive(true);
            }

            scoreScreenFailed[0].SetActive(true);
        }


    }


    // ---- C'EST DEGEULASSE ----- // (mais ça marche)
    IEnumerator TimerUI(int index)
    {
        yield return new WaitForSeconds(0.1f);

        scoreScreenUICanvas[index].enabled = true;
    }

    #region Anim Trigger Condition
    // ---- Trigger des anims pour les conditions ---- //
    public void TimerConditionFailed()
    {
        timerFailedAnim.Play("A_Timer_Condition_Failed");
    }

    public void ScoreConditionCompleted()
    {
        scoreCompletedAnim.Play("A_Score_Condition_Completed");
    }
    #endregion

}