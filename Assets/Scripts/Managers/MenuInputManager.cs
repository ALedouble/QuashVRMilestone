﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuInputManager : MonoBehaviour
{
    public bool rightHandIsActive = true;
    public void OnRightHandTriggerPress()
    {
        if(rightHandIsActive == false)
        {
            SwitchActiveHand();
        }
    }

    public void OnLeftHandTriggerPress()
    {
        if (rightHandIsActive == true)
        {
            SwitchActiveHand();
        }
    }

    public void OnRightHandTriggerRelease()
    {
        
    }

    public void OnLeftHandTriggerRelease()
    {
        
    }

    private void SwitchActiveHand()
    {
        rightHandIsActive = !rightHandIsActive;

        SetRightPointerActive(rightHandIsActive);
        SetLeftPointerActive(!rightHandIsActive);
    }

    private void SetRightPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_StraightPointerRenderer>().enabled = activeState;
    }

    private void SetLeftPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_UIPointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_StraightPointerRenderer>().enabled = activeState;
    }
}