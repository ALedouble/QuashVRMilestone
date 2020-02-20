using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelRecap : MonoBehaviour
{
    public TextMeshProUGUI levelTitle;

    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI bestCombo;

    public TextMeshProUGUI[] conditionType;
    public TextMeshProUGUI[] conditionReachedAt;

    public SpriteRenderer[] stars;
}
