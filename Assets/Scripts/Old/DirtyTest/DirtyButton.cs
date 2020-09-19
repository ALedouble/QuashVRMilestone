using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DirtyButton : MonoBehaviour
{
    public bool isGood;
    public Image image;
    public DirtyPlaytestManager manager;
    public TextMeshProUGUI textMesh;

    public void SetButton(bool p_good,Color p_color,string p_text)
    {
        isGood = p_good;
        image.color = p_color;
        textMesh.text = p_text;
    }

    public void PushButton()
    {
        if (isGood)
        {
            manager.GoodAnswer();
        }
        else
        {
            manager.BadAnswer();
        }
    }
}
