using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RoomName : MonoBehaviour
{
    public TextMeshProUGUI text;
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [PunRPC]
    void RoomTitle()
    {
        text.text = CreateRoom.Instance._roomName.text;
    }

    // Update is called once per frame
    void Update()
    {
        photonView.RPC("RoomTitle", RpcTarget.All);

        if (text.text != CreateRoom.Instance._roomName.text)
        {
            photonView.RPC("RoomTitle", RpcTarget.All);
        }
    }

}
