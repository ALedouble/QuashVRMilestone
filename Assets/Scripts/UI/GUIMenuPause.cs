using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMenuPause : MonoBehaviour
{
    #region Singleton
    public static GUIMenuPause guiMenuPause;


    #endregion

    private void Awake()
    {
        guiMenuPause = this;
        gameObject.SetActive(false);
    }

    public void GamePaused()
    {
        gameObject.SetActive(true);
    }

    public void GameResumed()
    {
        gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }
}
