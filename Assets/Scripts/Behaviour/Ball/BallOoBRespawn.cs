using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOoBRespawn : MonoBehaviour
{
    public float killHeight;

    void Update()
    {
        if(GameManager.Instance.offlineMode || BallMultiplayerBehaviour.Instance.IsBallOwner)
        {
            if (transform.position.y < killHeight)
            {
                BallManager.instance.RespawnBall();
            }
        }
    }
}
