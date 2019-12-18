using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainHand
{
    RIGHTHANDED,
    LEFTHANDED
}

public class GameplayInputManager : IInputable
{
    //Player settings (Gaucher/Droitier, pouvoirs utilises, ...)
    public MainHand mainHand = MainHand.RIGHTHANDED;

    public void OnRightTriggerPress()
    {
        if(mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
            //Anime de malade!!!
        }
    }

    public void OnLeftTriggerPress()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
            //Anime de malade!!!
        }
    }

    public void OnRightTriggerRelease()
    {
        if (mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
            //Anime bof mais bien quand meme, hein!!!
        }
    }

    public void OnLeftTriggerRelease()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
            //Anime bof mais bien quand meme, hein!!!
        }
    }

    //Ajouter les supers
}
