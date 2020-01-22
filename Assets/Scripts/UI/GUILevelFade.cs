using UnityEngine;
using UnityEngine.SceneManagement;

public class GUILevelFade : MonoBehaviour
{
    public Animator animFade;
    private int levelToLoad;

    #region Singleton
    public static GUILevelFade instance;

    public void Awake()
    {
        instance = this;
    }
    #endregion

    public void FadeToLevel (int levelIndex)
    {
        levelToLoad = levelIndex;
        animFade.SetTrigger("FadeOut");
    }

    public void FadeToNextLevel()
    {
        FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
