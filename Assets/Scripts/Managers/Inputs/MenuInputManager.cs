using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuInputManager : IInputable
{
    //Ajouter preference Droitier Gaucher
    public bool rightHandIsActive = true;

    public void EnterInputMod()
    {
        //Ajouter preference Droitier Gaucher
        rightHandIsActive = true;

        SetRightPointerActive(rightHandIsActive);
        SetLeftPointerActive(!rightHandIsActive);
    }
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
        //Debug.LogError("RightPointer State : " + QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().IsPointerActive());
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(activeState);
        //Debug.LogError("RightPointer State : " + QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().IsPointerActive());
    }

    private void SetLeftPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().Toggle(activeState);
    }
}
