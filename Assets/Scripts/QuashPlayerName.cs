using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuashPlayerName
{
    private static Viveport.StatusCallback ViveportCallback;

    public static string UserName 
    {
        get
        {
            switch(BuildPlatformManager.Instance.targetBuildPlatform)
            {
                case TargetBuildPlatform.Dev:
                    return "GOD";
                    break;
                case TargetBuildPlatform.Steam:
                    if (SteamManager.Initialized)
                        return SteamFriends.GetPersonaName();
                    break;
                case TargetBuildPlatform.Viveport:
                    if (ViveportCallback == null)
                        ViveportCallback += (viveportInt => { });
                    if (Viveport.User.IsReady(ViveportCallback) == 0)
                        return Viveport.User.GetUserName();
                    break;
            }

            return "WTF Alexis";
        }
    }
}
