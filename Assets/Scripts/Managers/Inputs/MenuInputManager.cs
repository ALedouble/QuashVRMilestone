using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuInputManager : IInputable
{
    //Ajouter preference Droitier Gaucher
    public bool rightHandIsActive = false;
    public bool leftHandIsActive = false;

    public void EnterInputMod()
    {
        //Ajouter preference Droitier Gaucher
        rightHandIsActive = false;
        leftHandIsActive = false;

        SetRightPointerActive(true);
        //SetRightPointerActive(rightHandIsActive);
        //SetLeftPointerActive(!rightHandIsActive);
    }

    public void ExitInputMod()
    {
        rightHandIsActive = false;
        leftHandIsActive = false;

        SetRightPointerActive(false);
        SetLeftPointerActive(false);
    }

    public void OnRightTriggerPress()
    {
        if(rightHandIsActive == false)
        {
            //SwitchActiveHand();
            SetRightPointerActive(true);
            rightHandIsActive = true;
        }
    }

    public void OnLeftTriggerPress()
    {
        if (leftHandIsActive == false)
        {
            //SwitchActiveHand();
            SetLeftPointerActive(true);
            leftHandIsActive = true;
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
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(activeState);
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = true;
    }

    private void SetLeftPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().Toggle(activeState);
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_UIPointer>().enabled = true;
    }
}
