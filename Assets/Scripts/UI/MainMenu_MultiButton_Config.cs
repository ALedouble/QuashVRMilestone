﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu_MultiButton_Config : MonoBehaviour
{
    [SerializeField] Button button;
    private string sceneName = "Scene_Menu";


    private void Awake()
    {
        button.onClick.AddListener(() => LoadingScene());
    }


    public void LoadingScene()
    {
        GUILevelFade.instance.FadeOut();
        StartCoroutine(AnimFade());
    }

    public void LoadingSceneToCampaign()
    {
        JSON.instance.currentLevelFocused = LevelManager.instance.currentLevel;

        JSON.instance.isGoingStraightToCampaign = true;
        LoadingScene();
    }

    IEnumerator AnimFade()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}
