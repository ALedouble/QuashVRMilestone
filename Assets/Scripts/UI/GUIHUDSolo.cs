using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHUDSolo : MonoBehaviour
{
    [Header("HUDs")]
    [SerializeField] GameObject withCondition;
    [SerializeField] GameObject withoutCondition;

    [Header("References")]
    [SerializeField] GUIScoreConditionData scoreConditionData;
    [SerializeField] GUIScoreData scoreData;
    [SerializeField] GUITimerData timerData;

    #region Singleton
    public static GUIHUDSolo instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    public void ActivateWithCondition()
    {
        withCondition.SetActive(true);
    }

    public void ActivateWithoutCondition()
    {
        withoutCondition.SetActive(true);
    }

    public void TimerConditionCompleted()
    {

    }

}
