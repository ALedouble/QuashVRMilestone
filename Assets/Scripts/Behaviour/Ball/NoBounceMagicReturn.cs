using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBounceMagicReturn : MonoBehaviour
{
    private float absoluteZVelocity;
    private float gravity;
    private float absolteXAcceleration;

    private float xAcceleration;
    private float zVelocity;


    public NoBounceMagicReturn(float absoluteZVelocity, float gravity, float absoluteXAcceleration=0)
    {
        this.absoluteZVelocity = absoluteZVelocity;
        this.gravity = gravity;
        this.absolteXAcceleration = absoluteXAcceleration; 
    }

    public Vector3 CalculateNewVelocity(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        if (targetPosition.x - ballImpactPosition.x <= 0)
            xAcceleration = absolteXAcceleration;
        else
            xAcceleration = -absolteXAcceleration;

        if (targetPosition.z - ballImpactPosition.z <= 0)
            zVelocity = -absoluteZVelocity;
        else
            zVelocity = absoluteZVelocity;


        float T = (targetPosition.z - ballImpactPosition.z) / zVelocity;

        float yVelocity = (targetPosition.y - ballImpactPosition.y + gravity * T * T / 2f) / T;
        float xVelocity = (targetPosition.x - ballImpactPosition.x) / T;
        //float xVelocity = (targetPosition.x - ballImpactPosition.x - xAcceleration * T * T / 2f) / T;

        return new Vector3(xVelocity, yVelocity, zVelocity);
    }
}
