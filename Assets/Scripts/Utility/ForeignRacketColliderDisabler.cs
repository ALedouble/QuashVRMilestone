using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ForeignRacketColliderDisabler : MonoBehaviour
{
    private void Start()
    {
       if(!(gameObject.GetComponent<PhotonView>().IsMine))
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
