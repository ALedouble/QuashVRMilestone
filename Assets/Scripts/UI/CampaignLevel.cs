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
    // Start is called before the first frame update
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
}
