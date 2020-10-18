using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordPresenceMarker : MonoBehaviour
{
    public GameSituation gameState;
    public string situationDetails;

    private void OnEnable()
    {
        DiscordManager.Instance?.SetDiscordPresence(gameState, situationDetails);
    }
}
