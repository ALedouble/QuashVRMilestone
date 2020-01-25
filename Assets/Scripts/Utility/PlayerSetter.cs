using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VRTK;

public class PlayerSetter : MonoBehaviour
{
    void Start()
    {
        if(!GetComponent<PhotonView>().IsMine)
        {
            GameObject leftController = GetComponentInChildren<LeftControllerGetter>().gameObject;
            GameObject rightController = GetComponentInChildren<RightControllerGetter>().gameObject;

            leftController.GetComponent<VRTK_Pointer>().enabled = false;
            rightController.GetComponent<VRTK_Pointer>().enabled = false;
        }
    }
}
