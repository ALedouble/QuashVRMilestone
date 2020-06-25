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

    public List<GameObject> lockImages;
    public List<GameObject> unlockImages;
    public List<GameObject> doneImages;
    public List<GameObject> fullStarsImages;
}
