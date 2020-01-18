using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LayerBehaviour : MonoBehaviour
{
    public GameObject layerCompleteObj;
    public Animator animLayerCompleted;


    public void CompleteLayer()
    {
        //layerCompleteObj.SetActive(true);
        animLayerCompleted.Play("A_HUD_Layer_Completed");
    }
}
