﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugString : MonoBehaviour
{
    public TextMeshProUGUI serverName;
    public TextMeshProUGUI regionName;
    // Start is called before the first frame update
    void Start()
    {
        //serverName.text = PhotonNetwork.CurrentRoom.Name;
        //regionName.text = PhotonNetwork.CloudRegion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}