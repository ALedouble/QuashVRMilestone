using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSetUp : MonoBehaviour
{
    public GameObject campaignNewScreenGO;
    public GameObject titleCampaignGO;
    public GameObject mainScreenGo;
    public GameObject firstTimeGo;
    public Animator animToPlay;

    void Start()
    {
        if (JSON.instance.isGoingStraightToCampaign)
        {
            JSON.instance.isGoingStraightToCampaign = false;
            GoToCampaign();
        }
        else
        {
            if (!PlayerSettings.Instance.HadDominantHandWarning)
            {
                firstTimeGo.SetActive(true);

                PlayerSettings.Instance.HadDominantHandWarning = true;
            }
            else
            {
                mainScreenGo.SetActive(true);
            }
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
