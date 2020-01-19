using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBounceMagicReturn : MonoBehaviour
{
    private float zVelocity;
    private float gravity;
    private float absolteXAcceleration;

    private float xAcceleration;


    public NoBounceMagicReturn(float zVelocity, float gravity, float absoluteXAcceleration)
    {
        this.zVelocity = zVelocity;
        this.gravity = gravity;
        this.absolteXAcceleration = absoluteXAcceleration; 
    }

    public Vector3 CalculateNewVelocity(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        if (targetPosition.x - ballImpactPosition.x <= 0)
            xAcceleration = absolteXAcceleration;
        else
            xAcceleration = -absolteXAcceleration;


        float T = (targetPosition.z - ballImpactPosition.z) / zVelocity;

        float yVelocity = (targetPosition.y - ballImpactPosition.y + gravity * T * T) / T;
        float xVelocity = (targetPosition.x - ballImpactPosition.x - xAcceleration * T * T) / T;

        return new Vector3(xVelocity, yVelocity, zVelocity);
    }
}
