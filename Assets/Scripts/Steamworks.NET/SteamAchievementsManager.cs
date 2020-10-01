using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievementsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (!SteamManager.Initialized)
            return;

        //SteamUserStats.SetAchievement("QS_WIN_FIRSTLEVEL");
        //SteamUserStats.SetAchievement("QS_WIN_EVERYLEVEL");
        //SteamUserStats.SetAchievement("QS_COLLECT_30PERCENT_STARS");
        //SteamUserStats.SetAchievement("QS_COLLECT_60PERCENT_STARS");
        //SteamUserStats.SetAchievement("QS_COLLECT_100PERCENT_STARS");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
