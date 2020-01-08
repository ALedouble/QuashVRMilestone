using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketImpactFX : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    GameObject playerRacket;
    
    // Start is called before the first frame update
    void Start()
    {
        BallEventManager.instance.OnCollisionWithRacket += PlayEffect;
        playerRacket = RacketManager.instance?.localPlayerRacket;
    }

    void PlayEffect()
    {
        transform.position = playerRacket.transform.position;
        particleSystem.Play();
    }
}
