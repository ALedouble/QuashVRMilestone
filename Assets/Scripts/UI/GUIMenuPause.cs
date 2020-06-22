using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMenuPause : MonoBehaviour
{
    #region Singleton
    public static GUIMenuPause guiMenuPause;


    #endregion

    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        guiMenuPause = this;
        IsGamePaused = false;
        Debug.Log("GUIMenuPause Awake!");
    }

    public void GamePaused()
    {
        IsGamePaused = true;
        gameObject.SetActive(true);
        Time.timeScale = 0;
        PlayerInputManager.instance.SetInputMod(InputMod.MENU);
    }

    public void GameResumed()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        PlayerInputManager.instance.SetInputMod(InputMod.GAMEPLAY);
        IsGamePaused = false;
    }
}
