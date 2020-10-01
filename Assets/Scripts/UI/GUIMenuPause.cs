using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMenuPause : MonoBehaviour
{
    #region Singleton
    public static GUIMenuPause guiMenuPause;
    #endregion

    [Header("Multiplayer Only")]
    public GameObject PauseMenuP1;
    public GameObject PauseMenuP2;

    private void Awake()
    {
        guiMenuPause = this;
        gameObject.SetActive(false);
    }

    public void GamePaused()
    {
        if(GameManager.Instance.offlineMode)
        {
            gameObject.SetActive(true);
        }
        else
        {
            if(QPlayerManager.instance.LocalPlayerID == QPlayer.PLAYER1)
            {
                PauseMenuP1.SetActive(true);
            }
            else
            {
                PauseMenuP2.SetActive(true);
            }
        }

        AudioManager.instance.PlaySound("Open_Pause", Vector3.zero);
    }

    public void GameResumed()
    {
        if (GameManager.Instance.offlineMode)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (QPlayerManager.instance.LocalPlayerID == QPlayer.PLAYER1)
            {
                PauseMenuP1.SetActive(false);
            }
            else
            {
                PauseMenuP2.SetActive(false);
            }
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        AudioManager.instance.PlaySound("Close_Pause", Vector3.zero);
    }
}
