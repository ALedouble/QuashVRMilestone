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
        Debug.Log("A: " + A);
        double B = CalculateB(ballImpactPosition, targetPosition);
        Debug.Log("B: " + B);
        double C = CalculateC(ballImpactPosition, targetPosition);
        Debug.Log("C: " + C);
        double D = CalculateD(ballImpactPosition, targetPosition);
        Debug.Log("D: " + D);

        double E = CalculateE(A);
        Debug.Log("E: " + E);
        double F = CalculateF(B, C);
        Debug.Log("F: " + F);
        double G = CalculateG(ballImpactPosition, A, D);
        Debug.Log("G: " + G);

        double H = CalculateH(B, E, F);
        Debug.Log("H: " + H);
        double I = CalculateI(ballImpactPosition, B, E ,F, G);
        Debug.Log("I: " + I);
        double J = CalculateJ(ballImpactPosition, B, E, F, G);
        Debug.Log("J: " + J);
        double K = CalculateK(ballImpactPosition, B, G);
        Debug.Log("K: " + K);

        double P = CalculateP(H, I, J);
        Debug.Log("P: " + P);
        double Q = CalculateQ(H, I, J, K);
        Debug.Log("Q: " + Q);
        double tOne = CalculateTOne(P, Q);
        Debug.Log("tOne: " + tOne);

        double yVelocity = CalculateYVelocity(tOne, H, I);

        double T12 = CalculateT12(yVelocity, ballImpactPosition);
        double Tt = CalculateTt(T12, zVelocity, ballImpactPosition, targetPosition);

        double xVelocity = CalculateXVelocity(ballImpactPosition, targetPosition, yVelocity, T12, Tt);

        Debug.Log("Velocity :: x: " + xVelocity + "  y: " + yVelocity + "  z: " + zVelocity);
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
        return (J * J - I * I) / (3 * H * H);
    }

    private double CalculateQ(double H, double I, double J, double K)
    {
        return -I * I * I / (3 * 3 * 3 * H * H * H) + I * I / (3 * 3 * H * H) + J * I / (3 * H * H) + K / H;
    }

    private double CalculateTOne(double P, double Q)
    {
        double double1 = (-Q - Mathf.Sqrt( (float)(Q * Q + 4 * P * P * P / 27) )) / 2;
        double double2 = (-Q + Mathf.Sqrt( (float)(Q * Q + 4 * P * P * P / 27)) ) / 2;
        return Mathf.Pow((float)double1, 1f / 3f) + Mathf.Pow((float)double2, 1f / 3f);
    }

    private double CalculateYVelocity(double tOne, double H, double I)
    {
        return tOne - I / (3 * H);
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
}
