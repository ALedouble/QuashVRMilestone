using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    private PhotonView photonView;
    [HideInInspector] public bool isThereTimerCondition;
    [HideInInspector] public float timerConditionValue;
    private bool hasTimerConditionFailed = false;


    public delegate void TimeEvent();
    public event TimeEvent OnTimerEnd;


    public float timerSpeedModifier = 1f;
    private float currentTimer;
    public float CurrentTimer
    {
        get => currentTimer;
        set
        {
            currentTimer = value;
            timerHasChanged = true;
        }
    }

    public float LevelMaxTime { get; set; }
    public bool IsTimeFlying { get; private set; }

    private GUITimerData timerGUI;
    private int minutes;
    private int seconds;

    private bool timerHasChanged = true;


    private void Awake()
    {
        Instance = this;

        photonView = GetComponent<PhotonView>();

        IsTimeFlying = false;
        currentTimer = 0f;
        //LevelMaxTime = 0f;
    }

    private void Update()
    {
        if (timerHasChanged)
            UpdateTimeText();
    }

    private void FixedUpdate()
    {
        if (IsTimeFlying)
            UpdateTimer();
    }


    public void SetupTimerGUI(GUITimerData timerData)
    {
        timerGUI = timerData;
    }

    public void SetNewTimer(float newTimer = 0)
    {
        if (!LevelManager.instance.currentLevel.level.levelSpec.timeAttack)
        {
            CurrentTimer = 0;
            LevelMaxTime = newTimer;
        }
        else
        {
            CurrentTimer = newTimer;
            LevelMaxTime = newTimer;
        }
    }

    public void StartTimer()
    {
        IsTimeFlying = true;
    }

    public void StopTimer()
    {
        IsTimeFlying = false;
    }

    public void ResetTimer()
    {
        CurrentTimer = 0;
        //CurrentTimer = LevelMaxTime;
    }

    private void UpdateTimer()
    {
        if (!LevelManager.instance.currentLevel.level.levelSpec.timeAttack)
        {
            //Incrémente le timer
            CurrentTimer += Time.fixedDeltaTime * timerSpeedModifier;

            if (GameManager.Instance.offlineMode && !hasTimerConditionFailed)
            {
                if (isThereTimerCondition)
                {
                    if (CurrentTimer > timerConditionValue)
                    {
                        LevelManager.instance.playersHUD.TimerConditionFailed();
                        hasTimerConditionFailed = true;
                    }
                }
            }
        }
        else
        {
            //Décrémente le timer
            CurrentTimer -= Time.fixedDeltaTime * timerSpeedModifier;

            //No more time left
            if (currentTimer <= 0)
            {
                CurrentTimer = 0;
                IsTimeFlying = false;

                //Debug.Log("No TIME left");
                OnTimerEnd?.Invoke();
            }
        }
    }

    private void UpdateTimeText()
    {
        if (currentTimer < 0)
        {
            timerGUI.UpdateText("00:00");
            timerGUI.FillImage(0);
        }
        else
        {
            minutes = (int)currentTimer / 60;
            seconds = (int)currentTimer - (minutes * 60);

            if (minutes < 10)
            {
                if (seconds < 10)
                {
                    timerGUI.UpdateText("0" + minutes + ":" + "0" + seconds);
                }
                else
                {
                    timerGUI.UpdateText("0" + minutes + ":" + seconds);
                }
            }
            else
            {
                if (seconds < 10)
                {
                    timerGUI.UpdateText(minutes + ":" + "0" + seconds);
                }
                else
                {
                    timerGUI.UpdateText(minutes + ":" + seconds);
                }
            }

            if (isThereTimerCondition && currentTimer <= timerConditionValue)
            {
                float timerPercent = 1 - (currentTimer / timerConditionValue);

                //Décrémentation de l'UI (fill image) pour le timer 
                timerGUI.FillImage(timerPercent);
                //Debug.Log("Timer percent : " + timerPercent);
            }
            else
            {
                timerGUI.FillImage(0);
            }

        }
    }

    public void OnTimeAttackBoost()
    {
        int newTime = (int)CurrentTimer + (int)LevelManager.instance.currentLevel.level.levelSpec.timePerLayer;
        SetNewTimer(newTime);
    }
}
