using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public class DiscordManager : MonoBehaviour
{
    public static DiscordManager Instance { get; private set; }

    public Discord.Discord discord { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDiscordSDK();
    }

    private void OnApplicationQuit()
    {
        DisconnetDiscordSDK();
    }

    private void Update()
    {
        discord.RunCallbacks();
    }

    public void InitializeDiscordSDK()
    {
        discord = new Discord.Discord(766943568107274250, (UInt64)Discord.CreateFlags.Default);
    }

    public void DisconnetDiscordSDK()
    {
        discord.Dispose();
    }

    public void SetDiscordPresence(GameSituation situation)
    {
        var activity = new Discord.Activity();

        switch(situation)
        {
            case GameSituation.Lobby :
                activity.State = "In the Lobby";
                break;
            case GameSituation.Solo:
                activity.State = "In the Campaign";
                break;
            case GameSituation.Multi:
                activity.State = "In Multiplayer Match";
                break;
        }

        UpdateDiscordPresence(activity);
    }

    private void UpdateDiscordPresence(Discord.Activity activity)
    {
        var activityManager = discord.GetActivityManager();
        
        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {
                Debug.Log("Discord Presence Updated!");
            }
        });
    }
}
