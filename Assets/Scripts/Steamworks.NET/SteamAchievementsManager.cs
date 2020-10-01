using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievementsManager : MonoBehaviour
{
    int totalStars = 0;
    int playerStars = 0;

    public SteamAchievementsManager instance;

    void Awake()
    {
        if (!SteamManager.Initialized)
            return;

        instance = this;

        //SteamUserStats.SetAchievement("QS_WIN_FIRSTLEVEL");
        //SteamUserStats.SetAchievement("QS_WIN_EVERYLEVEL");
        //SteamUserStats.SetAchievement("QS_COLLECT_30PERCENT_STARS");
        //SteamUserStats.SetAchievement("QS_COLLECT_60PERCENT_STARS");
        //SteamUserStats.SetAchievement("QS_COLLECT_90PERCENT_STARS");
        //SteamUserStats.SetAchievement("QS_COLLECT_ALL_STARS");
    }


    public void CheckAchievements()
    {
        for (int i = 0; i < Campaign.instance.levelsImplemented.Count; i++)
        {
            totalStars = totalStars + 1 + Campaign.instance.levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions;
        }


        

        float percent = Campaign.instance.PlayerStars / totalStars;
        if (percent == 1)
        {
            SteamUserStats.SetAchievement("QS_COLLECT_All_STARS");
        }

        if (percent >= 0.9f)
        {
            SteamUserStats.SetAchievement("QS_COLLECT_90PERCENT_STARS");
        }

        if (percent >= 0.6f)
        {
            SteamUserStats.SetAchievement("QS_COLLECT_60PERCENT_STARS");
        }

        if (percent >= 0.3f)
        {
            SteamUserStats.SetAchievement("QS_COLLECT_30PERCENT_STARS");
        }

        if (Campaign.instance.levelsImplemented[0].level.levelProgression.isDone)
        {
            SteamUserStats.SetAchievement("QS_WIN_FIRSTLEVEL");
        }


        bool areAllLevelsDone = true;
        for (int i = 0; i < Campaign.instance.levelsImplemented.Count; i++)
        {
            if(!Campaign.instance.levelsImplemented[i].level.levelProgression.isDone)
            {
                areAllLevelsDone = false;
                break;
            }
        }

        if(areAllLevelsDone)
            SteamUserStats.SetAchievement("QS_WIN_EVERYLEVEL");
    }
}
