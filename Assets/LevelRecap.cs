using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelRecap : MonoBehaviour
{
    public TextMeshProUGUI levelTitle;

    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI bestCombo;

    public TextMeshProUGUI[] conditionType;
    public TextMeshProUGUI[] conditionComparator;
    public TextMeshProUGUI[] conditionReachedAt;
    public TextMeshProUGUI levelCondition;

    public Image[] stars;

    public Button playButton;
}
