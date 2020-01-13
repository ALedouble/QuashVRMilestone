using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMenuPause : MonoBehaviour
{
    public GameObject menuPause;
    public static GUIMenuPause guiMenuPause;

    public void Awake()
    {
        guiMenuPause = this;
    }
    public void GamePaused()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameResumed()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
    }
}
