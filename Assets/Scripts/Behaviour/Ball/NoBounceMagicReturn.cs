using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBounceMagicReturn : MonoBehaviour
{
    float zVelocity;
    float gravity;
    float absolteXAcceleration;

    public NoBounceMagicReturn(float zVelocity, float gravity, float absoluteXAcceleration)
    {
        this.zVelocity = zVelocity;
        this.gravity = gravity;
        this.absolteXAcceleration = absoluteXAcceleration; 
    }
}
