using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBounceMagicReturn
{
    public double zVelocity;
    public double xAcceleration;

    public double gravity;
    public double bounciness;
    public double dynamicFriction;

    public double groundHeight;

    public OneBounceMagicReturn(float zVelocity, float xAcceleration, float gravity = 9.81f, float bounciness = 1f, float dynamicFriction = 0f, float groundHeight = 0f)
    {
        this.zVelocity = zVelocity;
        this.xAcceleration = xAcceleration;
        this.gravity = gravity;
        this.bounciness = bounciness;
        this.dynamicFriction = dynamicFriction;
        this.groundHeight = groundHeight;
    }

    public Vector3 CalculateNewVelocity(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        double A = CalculateA();
        //Debug.Log("A: " + A);
        double B = CalculateB(ballImpactPosition, targetPosition);
        //Debug.Log("B: " + B);
        double C = CalculateC(ballImpactPosition, targetPosition);
        //Debug.Log("C: " + C);
        double D = CalculateD(ballImpactPosition, targetPosition);
        //Debug.Log("D: " + D);

        double E = CalculateE(A);
        //Debug.Log("E: " + E);
        double F = CalculateF(B, C);
        //Debug.Log("F: " + F);
        double G = CalculateG(ballImpactPosition, A, D);
        //Debug.Log("G: " + G);

        double H = CalculateH(B, E, F);
        //Debug.Log("H: " + H);
        double I = CalculateI(ballImpactPosition, B, E ,F, G);
        //Debug.Log("I: " + I);
        double J = CalculateJ(ballImpactPosition, B, E, F, G);
        //Debug.Log("J: " + J);
        double K = CalculateK(ballImpactPosition, B, G);
        //Debug.Log("K: " + K);

        double P = CalculateP(H, I, J);
        //Debug.Log("P: " + P);
        double Q = CalculateQ(H, I, J, K);
        //Debug.Log("Q: " + Q);
        double T = CalculateT(P, Q);
        //Debug.Log("tOne: " + tOne);

        double yVelocity = CalculateYVelocity(T, H, I);

        double T12 = CalculateT12(yVelocity, ballImpactPosition);
        double Tt = CalculateTt(T12, zVelocity, ballImpactPosition, targetPosition);

        double xVelocity = CalculateXVelocity(ballImpactPosition, targetPosition, yVelocity, T12, Tt);

        Debug.Log("Velocity :: x: " + xVelocity + "  y: " + yVelocity + "  z: " + zVelocity);
        YVelocityVerification(yVelocity, H, I, J, K);

        Vector3 newVelocity = new Vector3((float)xVelocity, (float)yVelocity, (float)zVelocity);

        return newVelocity;
    }

    private double CalculateA()
    {
        return -gravity * (bounciness * (1 - dynamicFriction) + 1) / ((1 - dynamicFriction) * (1 - dynamicFriction));
    }

    private double CalculateB(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return gravity * (targetPosition.z - ballImpactPosition.z) * (2 + bounciness * (1 - dynamicFriction)) / (zVelocity * (1 - dynamicFriction) * (1 - dynamicFriction));
    }

    private double CalculateC(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return -bounciness * (targetPosition.z - ballImpactPosition.z) / (zVelocity * (1 - dynamicFriction));
    }

    private double CalculateD(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return -gravity * (targetPosition.z - ballImpactPosition.z) * (targetPosition.z - ballImpactPosition.z) / (zVelocity * zVelocity * (1 - dynamicFriction) * (1 - dynamicFriction)) + groundHeight - targetPosition.y;
    }

    private double CalculateE(double A)
    {
        return (2 * A * (1 - dynamicFriction) + bounciness * gravity) / (gravity * gravity * (1 - dynamicFriction));
    }

    private double CalculateF(double B, double C)
    {
        return B / gravity + C; 
    }

    private double CalculateG(Vector3 ballImpactPosition, double A, double D)
    {
        return 2 * (ballImpactPosition.y - groundHeight) * A / gravity + D;
    }

    private double CalculateH(double B, double E, double F)
    {
        return 2 * (E * B / gravity - E * F);
    }

    private double CalculateI(Vector3 BallImpactPosition, double B, double E, double F, double G)
    {
        return 2 * gravity * (BallImpactPosition.y - groundHeight) * E * E + B * B / (gravity * gravity) - F * F - 2 * E * G;
    }

    private double CalculateJ(Vector3 ballImpactPosition, double B, double E, double F, double G)
    {
        return 4 * (ballImpactPosition.y - groundHeight) * E * B - 2 * E * G;
    }

    private double CalculateK(Vector3 ballImpactPosition, double B, double G)
    {
        return 2 * B * B * (ballImpactPosition.y - groundHeight) / gravity + G * G;
    }

    private double CalculateP(double H, double I, double J)
    {
        return (3 * H * J - I * I) / (3 * H * H);
    }

    private double CalculateQ(double H, double I, double J, double K)
    {
        return -I * I * I / (3 * 3 * 3 * H * H * H) + I * I * I / (3 * 3 * H * H * H) + J * I / (3 * H * H) + K / H;
    }

    private double CalculateT(double P, double Q)
    {
        double delta = -(4 * P * P * P + 27 * Q * Q);
        Debug.Log("Delta: " + delta);
        if (delta < 0)
            return CalculateTDeltaNeg(delta, P, Q);
        else if (delta == 0)            //Attention Erreur numeriques
            return CalculateTDeltaZero(P, Q);
        else
            return CalculateTDeltaPos(P, Q);

    }

    private double CalculateTDeltaNeg(double delta, double P, double Q)
    {
        double uCube = (-Q + Mathf.Sqrt(-(float)delta / 27f)) / 2.0;
        double vCube = (-Q - Mathf.Sqrt(-(float)delta / 27f)) / 2.0;

        double u;
        double v;

        if (uCube < 0)
            u = -Mathf.Pow((float)(-uCube), 1f / 3f);
        else
            u = Mathf.Pow((float)uCube, 1f / 3f);

        if (vCube < 0)
            v = -Mathf.Pow((float)(-vCube), 1f / 3f);
        else
            v = Mathf.Pow((float)vCube, 1f / 3f);

        return u + v;
    }

    private double CalculateTDeltaZero(double P, double Q)
    {
        double tZero = 3 * Q / P;
        double tOne = -3 * Q / (2 * P);

        if (true)    // Trouver un critere
            return tZero;
        else
            return tOne;
    }

    private double CalculateTDeltaPos(double P, double Q)
    {
        double tZero = 2 * Mathf.Sqrt((float)(-P / 3.0)) * Mathf.Cos( Mathf.Acos( (float)(3 * Q * Mathf.Sqrt((float)(3 / -P)) / (2 * P)) ) );
        double tOne = 2 * Mathf.Sqrt((float)(-P / 3.0)) * Mathf.Cos( Mathf.Acos( (float)(3 * Q * Mathf.Sqrt((float)(3 / -P)) / (2 * P)) ) + 2 * Mathf.PI / 3f );
        double tTwo = 2 * Mathf.Sqrt((float)(-P / 3.0)) * Mathf.Cos( Mathf.Acos( (float)(3 * Q * Mathf.Sqrt((float)(3 / -P)) / (2 * P)) ) + 2 * Mathf.PI / 3f );

        if (true)
            return tZero;
        else
            return tOne;
    }

    private double CalculateYVelocity(double T, double H, double I)
    {
        return T - I / (3 * H);
    }

    private double CalculateT12(double yVelocity, Vector3 ballImpactPosition)
    {
        return (yVelocity + Mathf.Sqrt((float)yVelocity * (float)yVelocity + 2 * (float)gravity * (ballImpactPosition.y - (float)groundHeight))) / gravity;
    }

    private double CalculateTt(double T12, double zVelocity, Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return (targetPosition.z - ballImpactPosition.z - zVelocity * T12) / ((1 - dynamicFriction) * zVelocity);
    }

    private double CalculateXVelocity(Vector3 ballImpactPosition, Vector3 targetPosition, double yVelocity, double T12, double Tt)
    {
        if(targetPosition.x - ballImpactPosition.x < 0)  
            return (-(xAcceleration * Tt * Tt * (0.5f + (1 - dynamicFriction) * T12) + xAcceleration * T12 * T12 / 2f) + targetPosition.x - ballImpactPosition.x) / ((1 - dynamicFriction) * Tt * Tt + T12);
        else
            return (xAcceleration * Tt * Tt * (0.5f + (1 - dynamicFriction) * T12) + xAcceleration * T12 * T12 / 2f + targetPosition.x - ballImpactPosition.x) / ((1 - dynamicFriction) * Tt * Tt + T12);
    }

    private double CalculateError(double value, double H, double I, double J, double K)
    {
        double error = H * value * value * value + I * value * value + J * value + K;
        Debug.Log("YVelocity Error: " + error);

        return error;
    }

    private double SecantMethod4Order(double H, double I, double J, double K)
    {
        double xn = 0;
        double xnMinus1 = 0;
        double xnMinus2 = 0;
        double error = CalculateError(xn, H, I, J, K);
        while (error > 0.01)
        {
           
        }

        return xn;
    }
}
