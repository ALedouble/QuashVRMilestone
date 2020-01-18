using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LayerBehaviour : MonoBehaviour
{
    public GameObject layerCompleteObj;


    public void CompleteLayer()
    {
        layerCompleteObj.SetActive(true);
    }
}
