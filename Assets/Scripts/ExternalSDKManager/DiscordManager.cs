using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public class DiscordManager : MonoBehaviour
{
    public Discord.Discord discord { get; set; }

    private void Start()
    {
        InitializeDiscordSDK();
    }

    private void OnApplicationQuit()
    {
        DisconnetDiscordSDK();
    }

    public void InitializeDiscordSDK()
    {
        discord = new Discord.Discord(766943568107274250, (UInt64)Discord.CreateFlags.Default);

        UpdateDiscordPresence();
    }

    public void DisconnetDiscordSDK()
    {
        discord.Dispose();
    }

    private void UpdateDiscordPresence()
    {
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = "Testing",
            Details = "first activity Test"
        };

        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {
                Debug.LogError("Everything is fine!");
            }
        });
    }
}
