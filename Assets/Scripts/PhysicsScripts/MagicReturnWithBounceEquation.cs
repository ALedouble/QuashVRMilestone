using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicReturnWithBounceEquation : IMathFunction
{
    private float gravity;
    private float dynamicFriction;
    private float bounciness;

    private float vz0;

    private float yground;

    private float y0 = 0;
    private float z0 = 0;
    
    private float yt = 0;
    private float zt = 0;

    // calculate variable

    private float vz2;
    

    public MagicReturnWithBounceEquation(float gravity, float dynamicFriction, float bounciness, float vz0, float yground, float y0=0, float z0=0, float yt=0, float zt=0)
    {
        this.gravity = gravity;
        this.dynamicFriction = dynamicFriction;
        this.bounciness = bounciness;
        this.vz0 = vz0;
        this.yground = yground;

        this.y0 = y0;
        this.z0 = z0;
        this.yt = yt;
        this.zt = zt;
        
        vz2 = Vz2();
    }

    public void SetValues(float[] values)
    {
        if(values.Length == 4)
        {
            y0 = values[0];
            z0 = values[1];
            yt = values[2];
            zt = values[3];
        }
    }

    public float Evaluate(float vy0)
    {
        float t12 = T12(vy0);
        float zT12 = ZT12(t12);
        
        float vy2 = Vy2(vy0, t12);
        float tt = Tt(zT12, vz2);

        return -gravity * tt * tt / 2 + vy2 * tt + yground - yt;
    }

    private float T12(float vy0)
    {
        return vy0 + Mathf.Sqrt(vy0 * vy0 + 2 * gravity * (y0 - yground));
    }

    private float ZT12(float T12)
    {
        return vz0 * T12 + z0;
    }

    private float Vz2()
    {
        return (1 - dynamicFriction) * vz0;
    }

    private float Vy2(float vy0, float T12)
    {
        return -bounciness * (-gravity * T12 + vy0);
    }

    private float Tt(float zT12, float vz2)
    {
        return (zt - zT12) / vz2; 
    }
}
