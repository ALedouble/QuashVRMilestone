using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;

public class CampaignLevel : MonoBehaviour
{
    [HideInInspector] public int levelSelected;
    [HideInInspector] public LevelsScriptable levelScriptSelected;
    public LevelsScriptable levelToTest;
    [HideInInspector] public static int lastRecordedPanelIndex;


    public static CampaignLevel instance;




    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SelectLevel(int levelIndex)
    {
        GUILevelFade.instance.FadeOut();

        StartCoroutine(AnimFade());

        levelSelected = levelIndex;
    }

    public void SelectLevel(LevelsScriptable levelToPlay)
    {
        GUILevelFade.instance.FadeOut();

        StartCoroutine(AnimFade());

        levelScriptSelected = levelToPlay;
    }

    public void PlayTestLevel()
    {
        GUILevelFade.instance.FadeOut();

        StartCoroutine(AnimFade());

        levelScriptSelected = levelToTest;
    }
    IEnumerator AnimFade()
    {
        yield return new WaitForSeconds(1.5f);


        PhotonNetwork.LoadLevel(2);
    }
}
