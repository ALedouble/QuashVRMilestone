using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMenuPause : MonoBehaviour
{
    public GameObject menuPause;

    #region Singleton
    public static GUIMenuPause guiMenuPause;

    public void Awake()
    {
        guiMenuPause = this;
    }
    #endregion

    public void GamePaused()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        PlayerInputManager.instance.SetInputMod(InputMod.MENU);
    }

    public void GameResumed()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        PlayerInputManager.instance.SetInputMod(InputMod.GAMEPLAY);
    }
}
