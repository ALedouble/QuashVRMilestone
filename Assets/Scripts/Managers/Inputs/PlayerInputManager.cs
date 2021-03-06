﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public enum InputMod
{
    MENU = 0,
    GAMEPLAY = 1
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

    public float inputSetupDelay = 1f;
    public InputMod inputMod = InputMod.GAMEPLAY;
    public IInputable currentInput;

    public InputLinker LocalPlayerInputLinker { get; set; }

    private IInputable gameplayAction = new GameplayInputManager();
    private IInputable menuAction = new MenuInputManager();

    public void SetupInputMod()
    {
        StartCoroutine(DelayInputSetup());
    }

    private IEnumerator DelayInputSetup()
    {
        yield return new WaitForSeconds(inputSetupDelay);
        SetInputMod(inputMod);
    }

    public void SetInputMod(InputMod inputMod)
    {
        currentInput?.ExitInputMod();
        
        this.inputMod = inputMod;

        if (inputMod == InputMod.GAMEPLAY)
        {
            currentInput = gameplayAction;
        }
        else if (inputMod == InputMod.MENU)
        {
            currentInput = menuAction;
        }

        currentInput.EnterInputMod();
    }

    public InputMod GetInputMod()
    {
        return inputMod;
    }

    public void OnRightTriggerPress()
    {
        currentInput?.OnRightTriggerPress();
    }

    public void OnRightTriggerRelease()
    {
        currentInput?.OnRightTriggerRelease();
    }

    public void OnLeftTriggerPress()
    {
        currentInput?.OnLeftTriggerPress();
    }

    public void OnLeftTriggerRelease()
    {
        currentInput?.OnLeftTriggerRelease();
    }

    public void OnStartButtonPress()                    
    {
        currentInput?.OnStartButtonPress();
    }
}
