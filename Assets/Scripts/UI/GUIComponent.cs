using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GUIComponent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Image image;

    public virtual void UpdateText(string newText)
    {
        if (!textMesh) return;
        textMesh.text = newText;
    }

    public virtual void FillImage(float fillAmount)
    {
        if (!image) return;
        image.fillAmount = fillAmount;
    }
}
