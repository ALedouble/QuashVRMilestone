using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApparitionCollisionCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Racket")
        {
            BallManager.instance.BallApparitionBehaviour.BlockLoading();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Racket")
        {
            BallManager.instance.BallApparitionBehaviour.ResumeLoading();
        }
    }
}
