using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomName : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("helllllllllllllllllllllllo");
        text.text = CreateRoom.Instance._roomName.text;
        Debug.Log(text);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
