﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelRecap : MonoBehaviour
{
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI button;

    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI bestCombo;

    public TextMeshProUGUI[] conditionType;
    public TextMeshProUGUI[] conditionComparator;
    public TextMeshProUGUI[] conditionReachedAt;
    public TextMeshProUGUI levelCondition;

    public TextMeshProUGUI exoticCondition;

    [Space]

    public string bounceModeTitle;
    [TextArea] public string mandatoryBounceDescription;
    public string suddenDeathTitle;
    [TextArea] public string suddenDeathDescription;
    public string timeAttackTitle;
    public string timeAttackDescriptionBeforeValue;
    [TextArea] public string timeAttackDescriptionAfterValue;

    public Image[] stars;

    public Button playButton;
}
