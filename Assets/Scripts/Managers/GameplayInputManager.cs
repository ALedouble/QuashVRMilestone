using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainHand
{
    RIGHTHANDED,
    LEFTHANDED
}

public class GameplayInputManager : MonoBehaviour
{
    #region Singleton

    public static GameplayInputManager instance;

    private void Awake()
    {
        if (instance)
            Destroy(this);
        else
            instance = this;
    }
    #endregion

    //Player settings (Gaucher/Droitier, pouvoirs utilises, ...)
    public MainHand mainHand = MainHand.RIGHTHANDED;

    public void RightHandActionCall()
    {
        if(mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
            //Anime de malade!!!
        }
    }

    public void LeftHandActionCall()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.EnterEmpoweredState();
            //Anime de malade!!!
        }
    }

    public void RightHandActionEnd()
    {
        if (mainHand == MainHand.RIGHTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
            //Anime bof mais bien quand meme, hein!!!
        }
    }

    public void LeftHandActionEnd()
    {
        if (mainHand == MainHand.LEFTHANDED)
        {
            RacketManager.instance.ExitEmpoweredState();
            //Anime bof mais bien quand meme, hein!!!
        }
    }

    //Ajouter les supers
}
