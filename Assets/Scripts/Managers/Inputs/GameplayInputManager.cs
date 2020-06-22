using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public enum MainHand
{
    RIGHTHANDED,
    LEFTHANDED
}

public class GameplayInputManager : IInputable
{
    //Player settings (Gaucher/Droitier, pouvoirs utilises, ...)
    public MainHand mainHand = MainHand.RIGHTHANDED;

    public void EnterInputMod()
    {
        RacketManager.instance?.EnableRackets(true);

        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(false);
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = false;
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().Toggle(false);
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_UIPointer>().enabled = false;
    }

    public void ExitInputMod()
    {
        RacketManager.instance?.EnableRackets(false);
    }

    public void OnRightTriggerPress()
    {
        if(mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnLeftTriggerPress()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnRightTriggerRelease()
    {
        if (mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnLeftTriggerRelease()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.RacketAction();
        }
    }

    public void OnStartButtonPress()
    {
        GUIMenuPause.guiMenuPause.GamePaused();
    }
}
