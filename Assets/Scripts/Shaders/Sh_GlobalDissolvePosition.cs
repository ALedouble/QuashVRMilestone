using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sh_GlobalDissolvePosition : MonoBehaviour
{
    public static Transform ballTransform;

    private void Start()
    {
        Setup(); // Sera peut être appeler dans un manager plus tard
    }

    private void Update()
    {
        if(ballTransform)
            Shader.SetGlobalVector("_MagicalBallPos", ballTransform.position);
    }

    public static void Setup()
    {
        if (BallManager.instance)
            ballTransform = BallManager.instance.ball.transform;
    }
}
