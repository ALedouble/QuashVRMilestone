using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomTitle : MonoBehaviour
{
    public TextMeshProUGUI roomNameCreated;
    public TextMeshProUGUI roomNameDisplay;

    void Update()
    {
        DisplayRoomName();
    }

    void DisplayRoomName ()
    {
        roomNameDisplay.text = roomNameCreated.ToString();
    }
}
