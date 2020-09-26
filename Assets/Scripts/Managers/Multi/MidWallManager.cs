using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MidWallManager : MonoBehaviour
{
    public static MidWallManager Instance { get; private set; }

    private PhotonView photonView;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        photonView = GetComponent<PhotonView>();
    }

    public void SetMidWallStatus(bool isCollidable)
    {
        if (!GameManager.Instance.offlineMode)
        {
            if (isCollidable)
                photonView.RPC("ActivateMidWall", RpcTarget.All);
            else
                photonView.RPC("DisactivateMidWall", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ActivateMidWall()
    {
        if (LevelManager.instance.numberOfPlayers > 1)
        {
            //LevelManager.instance.midCollider.enabled = true;
            LevelManager.instance.midCollider.gameObject.SetActive(true);

        }

    }

    [PunRPC]
    private void DisactivateMidWall()
    {
        if (LevelManager.instance.numberOfPlayers > 1)
        {
            //LevelManager.instance.midCollider.enabled = false;
            LevelManager.instance.midCollider.gameObject.SetActive(false);
        }
    }
}

