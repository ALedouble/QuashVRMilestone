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
        
    }

    void PlayEffect(Collision collision)
    {
        playerRacket = RacketManager.instance?.localPlayerRacket;
        transform.position = playerRacket.transform.position;
        particleSystem.Play();
    }
}
