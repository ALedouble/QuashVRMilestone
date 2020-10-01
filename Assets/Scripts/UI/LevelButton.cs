using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header ("References")]
    public List<TextMeshProUGUI> buttonTexts;
    public Button button;
    public RectTransform rectTransform;

    public GameObject lockImages;
    public GameObject unlockImages;
    public GameObject doneImages;
    public GameObject fullStarsImages;

    public GameObject exoticUnlockImages;
    public GameObject exoticDoneImages;
}
