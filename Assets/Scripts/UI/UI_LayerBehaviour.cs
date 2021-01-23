using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LayerBehaviour : MonoBehaviour
{
    public GameObject layerCompleteObj;
    public Animator animLayerCompleted;

    /// <summary>
    /// Play UI icon animation for the completion of a layer
    /// </summary>
    public void CompleteLayer()
    {
        //layerCompleteObj.SetActive(true);
        animLayerCompleted.Play("A_HUD_Layer_Completed");
    }
}
