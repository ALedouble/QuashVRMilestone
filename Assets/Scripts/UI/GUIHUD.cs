using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHUD : MonoBehaviour
{
    [Header ("HUD References")]
    [SerializeField] GUIScoreData scoreData;
    [SerializeField] GUIComboData comboData;
    [SerializeField] GUIFillBarData fillBarData;
    [SerializeField] GUITimerData timerData;

    public GUIScoreData ScoreData { get => scoreData; }
    public GUIComboData ComboData { get => comboData; }
    public GUIFillBarData FillBarData { get => fillBarData; }
    public GUITimerData TimerData { get => timerData; }
}