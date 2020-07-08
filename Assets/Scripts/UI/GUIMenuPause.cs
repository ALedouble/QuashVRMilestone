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
        AudioManager.instance.PlaySound("Open_Pause", Vector3.zero);
    }

    public void GameResumed()
    {
        gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        AudioManager.instance.PlaySound("Close_Pause", Vector3.zero);
    }
}
