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

        PlayerInputManager.instance.LocalPlayerInputLinker.ControllerModelSetActive(true);
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
        //if(rightHandIsActive == false)
        //{
            //SwitchActiveHand();
            SetRightPointerActive(true);
            rightHandIsActive = true;
        //}
    }

    public void OnLeftTriggerPress()
    {
        //if (leftHandIsActive == false)
        //{
            //SwitchActiveHand();
            SetLeftPointerActive(true);
            leftHandIsActive = true;
        //}
    }

    public void OnRightTriggerRelease()
    {
        
    }

    public void OnLeftTriggerRelease()
    {
        
    }

    public void OnStartButtonPress()
    {
        if(GameManager.Instance.IsGamePaused)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    #region UtilityMethods

    private void SwitchActiveHand()
    {
        rightHandIsActive = !rightHandIsActive;

        SetRightPointerActive(rightHandIsActive);
        SetLeftPointerActive(!rightHandIsActive);
    }

    private void SetRightPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(activeState);
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = activeState;
    }

    private void SetLeftPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().Toggle(activeState);
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_UIPointer>().enabled = activeState;
    }

    #endregion
}
