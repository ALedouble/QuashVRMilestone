using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievementsManager : MonoBehaviour
{
    int totalStars = 0;
    int playerStars = 0;
    [HideInInspector] public bool Check_AchievementState;

    public static SteamAchievementsManager instance;

    void Awake()
    {
        if (!SteamManager.Initialized)
            return;

        instance = this;


        SteamUserStats.ResetAllStats(true);
        SteamUserStats.StoreStats();
    }


    public void CheckAchievements()
    {
        return;

        SteamUserStats.GetAchievement("QS_COLLECT_All_STARS", out Check_AchievementState);

        if (!Check_AchievementState)
        {
            for (int i = 0; i < Campaign.instance.levelsImplemented.Count; i++)
            {
                totalStars = totalStars + 1 + Campaign.instance.levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions;
            }


            float percent = (float)Campaign.instance.PlayerStars / (float)totalStars;
            Debug.Log("PlayerStars = " + Campaign.instance.PlayerStars + " <> " + totalStars + " percent => " + percent);


            if (percent == 1)
            {
                SteamUserStats.SetAchievement("QS_COLLECT_All_STARS");
            }


            SteamUserStats.GetAchievement("QS_COLLECT_90PERCENT_STARS", out Check_AchievementState);

            if (!Check_AchievementState)
            {
                Debug.Log("Verify _ QS_COLLECT_90PERCENT_STARS");
                if (percent >= 0.9f)
                {
                    SteamUserStats.SetAchievement("QS_COLLECT_90PERCENT_STARS");
                }
            }


            SteamUserStats.GetAchievement("QS_COLLECT_60PERCENT_STARS", out Check_AchievementState);

            if (!Check_AchievementState)
            {
                Debug.Log("Verify _ QS_COLLECT_60PERCENT_STARS");
                if (percent >= 0.6f)
                {
                    SteamUserStats.SetAchievement("QS_COLLECT_60PERCENT_STARS");
                }
            }


            SteamUserStats.GetAchievement("QS_COLLECT_30PERCENT_STARS", out Check_AchievementState);

            if (!Check_AchievementState)
            {
                Debug.Log("Verify _ QS_COLLECT_30PERCENT_STARS");
                if (percent >= 0.3f)
                {
                    SteamUserStats.SetAchievement("QS_COLLECT_30PERCENT_STARS");
                }
            }

        }


        SteamUserStats.GetAchievement("QS_WIN_FIRSTLEVEL", out Check_AchievementState);

        if (!Check_AchievementState)
        {
            Debug.Log("Verify _ QS_WIN_FIRSTLEVEL");
            if (Campaign.instance.levelsImplemented[0].level.levelProgression.isDone)
            {
                SteamUserStats.SetAchievement("QS_WIN_FIRSTLEVEL");
            }
        }


        SteamUserStats.GetAchievement("QS_WIN_EVERYLEVEL", out Check_AchievementState);

        if (!Check_AchievementState)
        {
            Debug.Log("Verify _ QS_WIN_EVERYLEVEL");

            bool areAllLevelsDone = true;
            for (int i = 0; i < Campaign.instance.levelsImplemented.Count; i++)
            {
                if (!Campaign.instance.levelsImplemented[i].level.levelProgression.isDone)
                {
                    areAllLevelsDone = false;
                    break;
                }
            }

            if (areAllLevelsDone)
                SteamUserStats.SetAchievement("QS_WIN_EVERYLEVEL");
        }


        SteamUserStats.StoreStats();
    }

    public void SetMultiplayerAchievement()
    {
        SteamUserStats.GetAchievement("QS_Play_One_Multi_Game", out Check_AchievementState);

        if (!Check_AchievementState)
        {
            SteamUserStats.SetAchievement("QS_Play_One_Multi_Game");
            SteamUserStats.StoreStats();
        }
    }

}
