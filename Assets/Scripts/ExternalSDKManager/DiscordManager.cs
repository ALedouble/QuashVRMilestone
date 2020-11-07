using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;
using System.Linq.Expressions;

public class DiscordManager : MonoBehaviour
{
    public static DiscordManager Instance { get; private set; }

    public Discord.Discord discord { get; set; }

    private bool isInitialized;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            if (!isInitialized)
                InitializeDiscordSDK();
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
        if(isInitialized)
            discord.RunCallbacks();
    }

    public void InitializeDiscordSDK()
    {
        Debug.Log("Discord SDK Init!");

        try
        {
            discord = new Discord.Discord(766943568107274250, (UInt64)Discord.CreateFlags.NoRequireDiscord);

            if (discord != null)
                isInitialized = true;
            else
                isInitialized = false;
        }
        catch (ResultException exception)
        {
            isInitialized = false;
        }
    }

    public void DisconnetDiscordSDK()
    {
        if(isInitialized)
            discord.Dispose();
    }

    public void SetDiscordPresence(GameSituation situation, string situationDetails = "")
    {
        if (!isInitialized)
            return;

        Debug.Log("Set Discord presence!");
        var activity = new Discord.Activity();

        switch (situation)
        {
            case GameSituation.MainMenu :
                activity.State = "Browsing Menus";
                activity.Details = "";
                break;
            case GameSituation.CampaignMenu:
                activity.State = "Browsing Campaign";
                activity.Details = "";
                break;
            case GameSituation.MultiplayerLobby:
                activity.State = "In a Multiplayer lobby";
                activity.Details = "";
                break;
            case GameSituation.Solo:
                activity.Details = "In the Campaign";
                activity.State = "Destroying blocks!";
                break;
            case GameSituation.Multi:
                activity.State = "In a Multiplayer Match";
                activity.Details = "";
                break;
        }

        activity.Assets.LargeImage = "quashlogo";
        activity.Assets.LargeText = "Quash";

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
