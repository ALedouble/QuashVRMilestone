using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GameplayInputManager : IInputable
{
    public void EnterInputMod()
    {
        RacketManager.instance?.EnableRackets(true);

        SetRightPointerActive(false);
        SetLeftPointerActive(false);

        PlayerInputManager.instance.LocalPlayerInputLinker.ControllerModelSetActive(false);
    }

    public void ExitInputMod()
    {
        RacketManager.instance?.EnableRackets(false);
    }

    public void OnRightTriggerPress()
    {
        if(PlayerSettings.Instance.PlayerDominantHand == PlayerHand.RIGHT)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnLeftTriggerPress()
    {
        if (PlayerSettings.Instance.PlayerDominantHand == PlayerHand.LEFT)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnRightTriggerRelease()
    {
        if (PlayerSettings.Instance.PlayerDominantHand == PlayerHand.RIGHT)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnLeftTriggerRelease()
    {
        if (PlayerSettings.Instance.PlayerDominantHand == PlayerHand.LEFT)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnStartButtonPress()
    {
        GameManager.Instance.PauseGame();
    }

    #region Utility

    private void SetRightPointerActive(bool activeState)
    {
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = activeState;
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().enabled = activeState;

        if (activeState)
            QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(true);
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
