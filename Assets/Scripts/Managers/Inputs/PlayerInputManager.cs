﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputMod
{
    MENU,
    GAMEPLAY
}

public class PlayerInputManager : MonoBehaviour
{
    #region Singleton
    public static PlayerInputManager instance;

    

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public InputMod inputMod = InputMod.GAMEPLAY;       //A cacher dans l'inspector

    private IInputable gameplayAction = new GameplayInputManager();
    private IInputable menuAction = new MenuInputManager();

    public void SetInputMod(InputMod inputMod)
    {
        this.inputMod = inputMod;

        if (inputMod == InputMod.GAMEPLAY)
        {
            gameplayAction.EnterInputMod();
        }
        else if (inputMod == InputMod.MENU)
        {
            menuAction.EnterInputMod();
        }
    }

    public InputMod GetInputMod()
    {
        return inputMod;
    }

    public void OnRightTriggerPress()
    {
        if(inputMod == InputMod.GAMEPLAY)
        {
            gameplayAction.OnRightTriggerPress();
        }
        else if(inputMod == InputMod.MENU)
        {
            menuAction.OnRightTriggerPress();
        }
    }

    public void OnRightTriggerRelease()
    {
        if (inputMod == InputMod.GAMEPLAY)
        {
            gameplayAction.OnRightTriggerRelease();
        }
        else if (inputMod == InputMod.MENU)
        {
            menuAction.OnRightTriggerRelease();
        }
    }

    public void OnLeftTriggerPress()
    {
        if (inputMod == InputMod.GAMEPLAY)
        {
            gameplayAction.OnLeftTriggerPress();
        }
        else if (inputMod == InputMod.MENU)
        {
            menuAction.OnLeftTriggerPress();
        }
    }

    public void OnLeftTriggerRelease()
    {
        if (inputMod == InputMod.GAMEPLAY)
        {
            gameplayAction.OnLeftTriggerRelease();
        }
        else if (inputMod == InputMod.MENU)
        {
            menuAction.OnLeftTriggerRelease();
        }
    }
}