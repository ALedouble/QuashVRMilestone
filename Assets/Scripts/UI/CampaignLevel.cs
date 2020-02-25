using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;

public class CampaignLevel : MonoBehaviour
{
    [HideInInspector]
    public int levelSelected;
    public LevelsScriptable levelScriptSelected;


    public static CampaignLevel Instance;


    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    public void SelectLevel(int levelIndex)
    {
        GUILevelFade.instance.FadeOut();

        StartCoroutine(AnimFade());

        levelSelected = levelIndex;
    }

    IEnumerator AnimFade()
    {
        yield return new WaitForSeconds(1.5f);


        PhotonNetwork.LoadLevel(2);
    }
}
