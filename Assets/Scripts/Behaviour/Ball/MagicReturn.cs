using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicReturn
{
    public double zVelocity;
    public double groundHeight;
    public double gravity;
    public double dynamicFriction;
    public double bounciness;

    public MagicReturn()
    {

    }

    public Vector3 CalculateNewVelocity(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        double A = CalculateA();
        double B = CalculateB(ballImpactPosition, targetPosition);
        double C = CalculateC(ballImpactPosition, targetPosition);
        double D = CalculateD(ballImpactPosition, targetPosition);

        double E = CalculateE(A);
        double F = CalculateF(B, C);
        double G = CalculateG(ballImpactPosition, A, D);

        double H = CalculateH(B, E, F);
        double I = CalculateI(ballImpactPosition, B, E ,F, G);
        double J = CalculateJ(ballImpactPosition, B, E, F, G);
        double K = CalculateK(ballImpactPosition, B, G);

        double P = CalculateP(H, I, J);
        double Q = CalculateQ(H, I, J, K);
        double tOne = CalculateTOne(P, Q);

        double yVelocity = CalculateYVelocity(tOne, H, I);
        double xVelocity = CalculateXVelocity(yVelocity);

        Vector3 newVelocity = new Vector3((float)xVelocity, (float)yVelocity, (float)zVelocity);

        return newVelocity;
    }

    private double CalculateA()
    {
        return -gravity * (bounciness * (1 - dynamicFriction) + 1) / ((1 - dynamicFriction) * (1 - dynamicFriction));
    }

    private double CalculateB(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return gravity * (targetPosition.x - ballImpactPosition.x) * (2 + bounciness * (1 - dynamicFriction)) / (zVelocity * (1 - dynamicFriction) * (1 - dynamicFriction));
    }

    private double CalculateC(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return -bounciness * (targetPosition.x - ballImpactPosition.x) / (zVelocity * (1 - dynamicFriction));
    }

    private double CalculateD(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return -gravity * (targetPosition.x - ballImpactPosition.x) * (targetPosition.x - ballImpactPosition.x) / (zVelocity * zVelocity * (1 - dynamicFriction) * (1 - dynamicFriction)) + groundHeight - targetPosition.y;
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
        double double1 = (-Q - Mathf.Sqrt((float)Q * (float)Q + 4 * (float)P * (float)P * (float)P / 27)) / 2;
        double double2 = (-Q + Mathf.Sqrt((float)Q * (float)Q + 4 * (float)P * (float)P * (float)P / 27)) / 2;
        return Mathf.Pow((float)double1, 1f / 3f) + Mathf.Pow((float)double2, 1f / 3f);
    }

    private double CalculateYVelocity(double tOne, double H, double I)
    {
        return tOne - I / (3 * H);
    }

    private double CalculateXVelocity(double yVelocity)
    {
        return 0;
    }
}
