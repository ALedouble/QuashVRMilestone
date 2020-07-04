using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuInputManager : IInputable
{
    private bool isLeftPointerInSight;
    private bool isRightPointerInSight;

    private bool shouldLeftPointerBeActive;
    private bool shouldRightPointerBeActive;


    public void EnterInputMod()
    {
        bool activeState = PlayerSettings.Instance.PlayerDominantHand == PlayerHand.LEFT;
        shouldLeftPointerBeActive = activeState;
        shouldRightPointerBeActive = !activeState;
            
        PlayerInputManager.instance.LocalPlayerInputLinker.ControllerModelSetActive(true);
    }

    public void ExitInputMod()
    {
        SetRightPointerActive(false);
        SetLeftPointerActive(false);
    }

    public void OnRightTriggerPress()
    {
        TryToSetRightPointerActive(true);
    }

    public void OnLeftTriggerPress()
    {
        TryToSetLeftPointerActive(true);
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

    #region Headset Glance

    public void OnLeftControllerGlanceEnter()
    {
        isLeftPointerInSight = true;

        if (shouldLeftPointerBeActive)
            SetLeftPointerActive(true);
    }

    public void OnLeftControllerGlanceExit()
    {
        isLeftPointerInSight = false;
        SetLeftPointerActive(false);
    }

    public void OnRightControllerGlanceEnter()
    {
        isRightPointerInSight = true;

        if (shouldRightPointerBeActive)
            SetRightPointerActive(true);
    }

    public void OnRightControllerGlanceExit()
    {
        isRightPointerInSight = false;
        SetRightPointerActive(false);
    }

    #endregion

    #region UtilityMethods

    private void TryToSetRightPointerActive(bool activeState)
    {
        shouldRightPointerBeActive = activeState;

        if(isRightPointerInSight)
        {
            SetRightPointerActive(activeState);
        }
    }

    private void SetRightPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().enabled = activeState;

        if (activeState)
            QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(true);
    }

    private void TryToSetLeftPointerActive(bool activeState)
    {
        shouldLeftPointerBeActive = activeState;
        
        if(isLeftPointerInSight)
        {
            SetLeftPointerActive(activeState);
        }
    }

    private void SetLeftPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_UIPointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().enabled = activeState;

        if (activeState)
            QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().Toggle(true);
    }

    #endregion
}
