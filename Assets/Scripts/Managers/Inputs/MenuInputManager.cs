using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuInputManager : IInputable
{
    public void EnterInputMod()
    {
        if (PlayerSettings.Instance.PlayerDominantHand == PlayerHand.LEFT)
            SetLeftPointerActive(true);
        else
            SetRightPointerActive(true);

        PlayerInputManager.instance.LocalPlayerInputLinker.ControllerModelSetActive(true);
    }

    public void ExitInputMod()
    {
        SetRightPointerActive(false);
        SetLeftPointerActive(false);
    }

    public void OnRightTriggerPress()
    {
        SetRightPointerActive(true);
    }

    public void OnLeftTriggerPress()
    {
        SetLeftPointerActive(true);
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

    private void SetRightPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(activeState);
        //QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().enabled = activeState;
    }

    private void SetLeftPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_UIPointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().Toggle(activeState);
        //QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().enabled = activeState;
    }

    #endregion
}
