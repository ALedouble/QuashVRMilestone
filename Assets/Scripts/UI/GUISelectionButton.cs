using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISelectionButton : GUIComponent
{
    [SerializeField] Button button;
    [SerializeField] Animator animator;
    bool buttonSelected;

    public void SelectButton(bool select)
    {
        buttonSelected = select;
        string trigger = select ? "Selected" : "Normal";
        animator?.SetTrigger(trigger);
    }

}
