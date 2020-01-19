using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHUD : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] GUIScoreData[] scoreData;
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
    [SerializeField] GameObject[] timerHUD;
    [SerializeField] GameObject[] layerCountHUD;

    public Transform[] layerCountParent;

    #region Singleton
    public static GUIHUD instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    public GUIScoreData[] ScoreData { get => scoreData; }
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
            animScoreScreen[i].Play("A_ScoreScreen_Appearing");

            scoreComboHUD[i].SetActive(false);
            timerHUD[i].SetActive(false);
            layerCountHUD[i].SetActive(false);

            maxComboData[i].UpdateText(ScoreManager.Instance.playersMaxCombo[i].ToString());
            maxScoreData[i].UpdateText(ScoreManager.Instance.score[i].ToString());
        }

        if (!GameManager.Instance.hasLost)
        {
            for (int i = 0; i < scoreScreenCompleted.Length; i++)
            {
                scoreScreenCompleted[i].SetActive(true);
                scoreScreenFailed[i].SetActive(false);
            }

        }
        else
        {
            for (int i = 0; i < scoreScreenFailed.Length; i++)
            {
                scoreScreenFailed[i].SetActive(true);
                scoreScreenCompleted[i].SetActive(false);
            }
        }
    }
}