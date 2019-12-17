using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputMod
{
    MENU,
    GAMEPLAY
}

public class PlayerInputManager : MonoBehaviour
{
    public InputMod inputMod = InputMod.GAMEPLAY;       //A cacher dans l'inspector

    public void SetInputMod(InputMod inputMod)
    {
        this.inputMod = inputMod;
    }

    public InputMod GetInputMod()
    {
        return inputMod;
    }

    public void OnRightTriggerPress()
    {
        if(inputMod == InputMod.GAMEPLAY)
        {
            GameplayInputManager.instance.RightHandActionCall();
        }
        else if(inputMod == InputMod.MENU)
        {
            //Ici Tristan
        }
    }

    public void OnRightTriggerRelease()
    {
        if (inputMod == InputMod.GAMEPLAY)
        {
            GameplayInputManager.instance.RightHandActionEnd();
        }
        else if (inputMod == InputMod.MENU)
        {
            //Ici Tristan
        }
    }

    public void OnLeftTriggerPress()
    {
        if (inputMod == InputMod.GAMEPLAY)
        {
            GameplayInputManager.instance.LeftHandActionCall();
        }
        else if (inputMod == InputMod.MENU)
        {
            //Ici Tristan
        }
    }

    public void OnLeftTriggerRelease()
    {
        if (inputMod == InputMod.GAMEPLAY)
        {
            GameplayInputManager.instance.LeftHandActionEnd();
        }
        else if (inputMod == InputMod.MENU)
        {
            //Ici Tristan
        }
    }
}
