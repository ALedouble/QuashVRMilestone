using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GUIComponent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Image image;

    public TextMeshProUGUI TextMesh { get => textMesh; }

    public virtual void UpdateText(string newText)
    {
        if (!textMesh) return;
        textMesh.text = newText;
    }

    public virtual void UpdateTextColor(Color32 newColor)
    {
        if (!textMesh) return;
        textMesh.color = newColor;
    }

    public virtual void FillImage(float fillAmount)
    {
        if (!image) return;
        image.fillAmount = fillAmount;
    }
}
