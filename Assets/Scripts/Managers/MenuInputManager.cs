using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuInputManager : IInputable
{
    public bool rightHandIsActive = true;
    public void OnRightTriggerPress()
    {
        if(rightHandIsActive == false)
        {
            SwitchActiveHand();
        }
    }

    public void OnLeftTriggerPress()
    {
        if (rightHandIsActive == true)
        {
            SwitchActiveHand();
        }
    }

    public void OnRightTriggerRelease()
    {
        
    }

    public void OnLeftTriggerRelease()
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
