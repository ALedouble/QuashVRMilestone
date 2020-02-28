using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITutorialText : MonoBehaviour
{
    [SerializeField] int startLayerIndex = 0;
    [SerializeField] int endLayerLayerIndex = 2;

    [SerializeField] Animator textAnimator;

    LevelManager levelManager;

    void Start()
    {
        levelManager = LevelManager.instance;
        levelManager.onLayerEndEvent += CheckLayer;
        CheckLayer();
    }

    void CheckLayer()
    {
        //Enable
        if(levelManager.currentLayer[0] == startLayerIndex)
        {
            textAnimator.SetTrigger("Appear");
        }

        //Disable
        if (levelManager.currentLayer[0] == endLayerLayerIndex)
        {
            textAnimator.SetTrigger("Disappear");
        }
    }
}
