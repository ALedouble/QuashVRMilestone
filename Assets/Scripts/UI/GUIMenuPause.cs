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

    public void GamePaused()
    {
        
        if(GameManager.Instance.offlineMode)
        {
            gameObject.SetActive(true);
            Debug.Log("Pause UI : " + gameObject.activeSelf);
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
            Debug.Log("Unpause UI " + gameObject.activeSelf); 
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
