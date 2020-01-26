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
        QPlayerManager.instance.GetLocalController(PlayerHand.RIGHT).GetComponent<VRTK_Pointer>().Toggle(false);
        QPlayerManager.instance.GetLocalController(PlayerHand.LEFT).GetComponent<VRTK_Pointer>().Toggle(false);
    }

    public void OnRightTriggerPress()
    {
        if(mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
        }
    }

    public void OnLeftTriggerPress()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
        }
    }

    public void OnRightTriggerRelease()
    {
        if (mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
        }
    }

    public void OnLeftTriggerRelease()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
        }
    }

    //Ajouter les supers
}
