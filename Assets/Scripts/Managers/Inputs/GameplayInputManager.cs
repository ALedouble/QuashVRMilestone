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
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_UIPointer>().enabled = false;
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().enabled = false;
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_StraightPointerRenderer>().enabled = false;

        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_UIPointer>().enabled = false;
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().enabled = false;
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_StraightPointerRenderer>().enabled = false;
    }

    public void OnRightTriggerPress()
    {
        if(mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
            //RacketManager.instance.SwitchRacketColor();
        }
    }

    public void OnLeftTriggerPress()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
            //RacketManager.instance.SwitchRacketColor();
        }
    }

    public void OnRightTriggerRelease()
    {
        if (mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
            //RacketManager.instance.EndSwitchRacketColor();
        }
    }

    public void OnLeftTriggerRelease()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
            //RacketManager.instance.EndSwitchRacketColor();
        }
    }

    //Ajouter les supers
}
