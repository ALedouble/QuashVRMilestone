using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveportAchievementManager : MonoBehaviour
{
    int totalStars = 0;
    int playerStars = 0;
    [HideInInspector] public bool Check_AchievementState;
    private Viveport.StatusCallback ViveportCallback;

    public static ViveportAchievementManager instance;

    void Awake()
    {
        if (!(BuildPlatformManager.Instance.targetBuildPlatform == TargetBuildPlatform.Viveport))
            return;

        instance = this;

        ViveportCallback += (viveportInt => { });
    }


    public void CheckAchievements()
    {
        Viveport.UserStats.DownloadStats(ViveportCallback);

        if (!Viveport.UserStats.GetAchievement("QS_COLLECT_All_STARS"))
        {
            for (int i = 0; i < Campaign.instance.levelsImplemented.Count; i++)
            {
                totalStars = totalStars + 1 + Campaign.instance.levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions;
            }


            float percent = (float)Campaign.instance.PlayerStars / (float)totalStars;
            //Debug.Log("PlayerStars = " + Campaign.instance.PlayerStars + " <> " + totalStars + " percent => " + percent);


            if (percent == 1)
            {
                Viveport.UserStats.SetAchievement("QS_COLLECT_All_STARS");
            }


            if (!Viveport.UserStats.GetAchievement("QS_COLLECT_90PERCENT_STARS"))
            {
                //Debug.Log("Verify _ QS_COLLECT_90PERCENT_STARS");
                if (percent >= 0.9f)
                {
                    Viveport.UserStats.SetAchievement("QS_COLLECT_90PERCENT_STARS");
                }
            }


            if (!Viveport.UserStats.GetAchievement("QS_COLLECT_60PERCENT_STARS"))
            {
                //Debug.Log("Verify _ QS_COLLECT_60PERCENT_STARS");
                if (percent >= 0.6f)
                {
                    Viveport.UserStats.SetAchievement("QS_COLLECT_60PERCENT_STARS");
                }
            }


            if (!Viveport.UserStats.GetAchievement("QS_COLLECT_30PERCENT_STARS"))
            {
                //Debug.Log("Verify _ QS_COLLECT_30PERCENT_STARS");
                if (percent >= 0.3f)
                {
                    Viveport.UserStats.SetAchievement("QS_COLLECT_30PERCENT_STARS");
                }
            }

        }


        if (!Viveport.UserStats.GetAchievement("QS_WIN_FIRSTLEVEL"))
        {
            //Debug.Log("Verify _ QS_WIN_FIRSTLEVEL");
            if (Campaign.instance.levelsImplemented[0].level.levelProgression.isDone)
            {
                Viveport.UserStats.SetAchievement("QS_WIN_FIRSTLEVEL");
            }
        }


        if (!Viveport.UserStats.GetAchievement("QS_WIN_EVERYLEVEL"))
        {
            //Debug.Log("Verify _ QS_WIN_EVERYLEVEL");

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
                Viveport.UserStats.SetAchievement("QS_WIN_EVERYLEVEL");
        }


        Viveport.UserStats.UploadStats(ViveportCallback);
    }

    public void SetMultiplayerAchievement()
    {
        if (!Viveport.UserStats.GetAchievement("QS_Play_One_Multi_Game"))
        {
            Viveport.UserStats.SetAchievement("QS_Play_One_Multi_Game");
            Viveport.UserStats.UploadStats(ViveportCallback);
        }
    }
}
