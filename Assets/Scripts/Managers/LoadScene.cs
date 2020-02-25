using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void LoadingScene ()
    {
        GUILevelFade.instance.FadeOut();
        StartCoroutine(AnimFade());
    }

    IEnumerator AnimFade()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}
