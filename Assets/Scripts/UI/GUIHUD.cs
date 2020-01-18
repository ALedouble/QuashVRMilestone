using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHUD : MonoBehaviour
{
    [Header ("HUD References")]
    [SerializeField] GUIScoreData[] scoreData;
    [SerializeField] GUIComboData[] comboData;
    [SerializeField] GUIFillBarData fillBarData;
    [SerializeField] GUITimerData timerData;

    [Header("HUD ScoreScreen References")]
    [SerializeField] GUIMaxScoreData maxScoreData;
    [SerializeField] GUIMaxComboData maxComboData;

    [Header("References")]
    [SerializeField] GameObject scoreScreen;
    [SerializeField] GameObject scoreScreenCompleted;
    [SerializeField] GameObject scoreScreenFailed;
    [SerializeField] GameObject scoreComboHUD;
    [SerializeField] GameObject timerHUD;
    [SerializeField] GameObject layerCountHUD;

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
    public GUIMaxScoreData MaxScoreData { get => maxScoreData; }
    public GUIMaxComboData MaxComboData { get => maxComboData; }

    // ----------------------------------------------------- //

   

}