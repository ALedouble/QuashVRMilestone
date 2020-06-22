using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSetUp : MonoBehaviour
{
    public GameObject campaignNewScreenGO;
    public GameObject titleCampaignGO;
    public Animator animToPlay;

    void Start()
    {
        if(JSON.instance.currentLevelFocused != null)
        {
            GoToCampaign();
        }
    }

    public void GoToCampaign()
    {
        campaignNewScreenGO.SetActive(true);
        titleCampaignGO.SetActive(true);
        animToPlay.Play("A_Window_Disappear");

        Campaign.instance.MoveCampaignPanelTo(Campaign.instance.GetPanelIndex(JSON.instance.currentLevelFocused));
    }
}
