using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIHUD : MonoBehaviour
{
    [Header("SOLO ONLY")]
    [SerializeField] GUIScoreConditionData scoreConditionData;
    [SerializeField] GUITimerConditionData timerConditionData;
    [SerializeField] GameObject[] conditionParents;
    [SerializeField] GameObject[] timerConditionParents;
    [SerializeField] Animator timerFailedAnim;
    [SerializeField] Animator scoreCompletedAnim;
    [SerializeField] GameObject[] completedStars;

    [Header("HUD References")]
    [SerializeField] GUIScoreData[] scoreDATAs;
    [SerializeField] GUIComboData[] comboData;
    [SerializeField] GUIFillBarData fillBarData;
    [SerializeField] GUITimerData timerData;

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
    [SerializeField] GameObject scoreScreenHUD;

    [SerializeField] VRTK.VRTK_UICanvas scoreScreenUICanvas;

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
    public GameObject[] ConditionParents { get => conditionParents; }

    public GUIComboData[] ComboData { get => comboData; }
    public GUIFillBarData FillBarData { get => fillBarData; }
    public GUITimerData TimerData { get => timerData; }
    public GUIMaxScoreData[] MaxScoreData { get => maxScoreData; }
    public GUIMaxComboData[] MaxComboData { get => maxComboData; }

    // ----------------------------------------------------- //

    public void EnableScoreScreen()
    {
        for (int i = 0; i < animScoreScreen.Length; i++)
        {
            scoreScreenHUD.SetActive(true);

            StartCoroutine(TimerUI());

            animScoreScreen[i].Play("A_ScoreScreen_Appearing");

            scoreComboHUD[i].SetActive(false);
            layerCountHUD[i].SetActive(false);

            maxComboData[i].UpdateText(ScoreManager.Instance.playersMaxCombo[i].ToString());
            maxScoreData[i].UpdateText(ScoreManager.Instance.score[i].ToString());

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
    IEnumerator TimerUI()
    {
        yield return new WaitForSeconds(0.1f);

        scoreScreenUICanvas.enabled = true;
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