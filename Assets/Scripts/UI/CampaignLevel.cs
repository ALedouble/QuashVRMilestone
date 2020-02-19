using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;

public class CampaignLevel : MonoBehaviour
{
    public static CampaignLevel Instance;

    [HideInInspector]
    public int levelSelected;
    public LevelsScriptable levelScriptSelected;


    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectLevel(int levelIndex){
        levelSelected = levelIndex;
        PhotonNetwork.LoadLevel(2);
    }

    public void SelectLevel(LevelsScriptable levelIndex)
    {
        PhotonNetwork.LoadLevel(2);
    }
}
