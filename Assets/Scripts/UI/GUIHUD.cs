using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHUD : MonoBehaviour
{
    [Header ("HUD References")]
    [SerializeField] GUIScoreData[] scoreData;
    [SerializeField] GUIComboData[] comboData;

    public GUIScoreData[] ScoreData { get => scoreData; }
    public GUIComboData[] ComboData { get => comboData; }

    private void Start()
    {
        
    }
}