using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameplayRestartButton : MonoBehaviour
{
    public void RestartGameplayScene()
    {
        GUILevelFade.instance.FadeOut();
        StartCoroutine(AnimFade());
    }

    IEnumerator AnimFade()
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.RestartScene();
    }
}
