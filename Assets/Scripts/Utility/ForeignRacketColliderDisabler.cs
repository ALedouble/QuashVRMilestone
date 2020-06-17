using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ForeignRacketColliderDisabler : MonoBehaviour
{
    private void Start()
    {
       if(!GameManager.Instance.offlineMode && !(gameObject.GetComponent<PhotonView>().IsMine))
       {
            //Debug.Log("Foreign Collider Deactivation + Set in Racket Manager");
            gameObject.GetComponent<BoxCollider>().enabled = false;
            RacketManager.instance.SetForeignPlayerRacket(gameObject);
       }
    }
}
