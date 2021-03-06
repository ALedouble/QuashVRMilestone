﻿using System.Collections;
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


    private double zUsedVelocity;



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
        if (targetPosition.z - ballImpactPosition.z < 0)
            zUsedVelocity = -zVelocity;
        else
            zUsedVelocity = zVelocity;

        double yVelocity = CalculateYVelocity(ballImpactPosition, targetPosition);
        double xVelocity = CalculateXVelocity(ballImpactPosition, targetPosition, yVelocity);

        Debug.Log("Velocity :: x: " + xVelocity + "  y: " + yVelocity + "  z: " + zUsedVelocity);

        return new Vector3((float)xVelocity, (float)yVelocity, (float)zUsedVelocity);
    }

    #region YVelocity Calculation

    private double CalculateYVelocity(Vector3 ballImpactPosition, Vector3 targetPosition)
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
        double I = CalculateI(ballImpactPosition, B, E, F, G);
        //Debug.Log("I: " + I);
        double J = CalculateJ(ballImpactPosition, B, E, F, G);
        //Debug.Log("J: " + J);
        double K = CalculateK(ballImpactPosition, B, G);
        Debug.Log("K: " + K);

       
        AnalyticSolution3OrderPolynom(H, I, J, K);
        return SecantMethod3Order(H, I, J, K);
    }

    #endregion

    #region Intermediate Calculation Methods

    private double CalculateA()
    {
        return -gravity * (2 * bounciness * (1 - dynamicFriction) + 1) / (2 * (1 - dynamicFriction) * (1 - dynamicFriction));
    }

    private double CalculateB(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return gravity * (targetPosition.z - ballImpactPosition.z) * (1 + bounciness * (1 - dynamicFriction)) / (zUsedVelocity * (1 - dynamicFriction) * (1 - dynamicFriction));
    }

    private double CalculateC(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return -bounciness * (targetPosition.z - ballImpactPosition.z) / (zUsedVelocity * (1 - dynamicFriction));
    }

    private double CalculateD(Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return -gravity * (targetPosition.z - ballImpactPosition.z) * (targetPosition.z - ballImpactPosition.z) / (2 * zUsedVelocity * zUsedVelocity * (1 - dynamicFriction) * (1 - dynamicFriction)) + groundHeight - targetPosition.y;
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
        return 4 * (ballImpactPosition.y - groundHeight) * E * B - 2 * F * G;
    }

    private double CalculateK(Vector3 ballImpactPosition, double B, double G)
    {
        return 2 * B * B * (ballImpactPosition.y - groundHeight) / gravity + G * G;
    }

    #endregion

    #region Analytic Solution

    private double AnalyticSolution3OrderPolynom(double H, double I, double J, double K)
    {
        double P = CalculateP(H, I, J);
        //Debug.Log("P: " + P);
        double Q = CalculateQ(H, I, J, K);
        //Debug.Log("Q: " + Q);
        double T = CalculateT(P, Q);
        //Debug.Log("tOne: " + tOne);

        double yVelocity = CalculateYV(T, H, I);
        CalculateError(yVelocity, H, I, J, K);

        return yVelocity;
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

    private double CalculateYV(double T, double H, double I)
    {
        return T - I / (3 * H);
    }

    #endregion

    #region XVelocity Calculation

    private double CalculateXVelocity(Vector3 ballImpactPosition, Vector3 targetPosition, double yVelocity)
    {
        double T12 = CalculateT12(yVelocity, ballImpactPosition);
        Debug.Log("T12: " + T12);
        double Tt = CalculateTt(T12, zUsedVelocity, ballImpactPosition, targetPosition);
        Debug.Log("Tt: " + Tt);

        if (targetPosition.x - ballImpactPosition.x < 0)
            return (-(xAcceleration * Tt * Tt * (0.5f + (1 - dynamicFriction) * T12) + xAcceleration * T12 * T12 / 2f) + targetPosition.x - ballImpactPosition.x) / ((1 - dynamicFriction) * Tt * Tt + T12);
        else
            return (xAcceleration * Tt * Tt * (0.5f + (1 - dynamicFriction) * T12) + xAcceleration * T12 * T12 / 2f + targetPosition.x - ballImpactPosition.x) / ((1 - dynamicFriction) * Tt * Tt + T12);
    }

    private double CalculateT12(double yVelocity, Vector3 ballImpactPosition)
    {
        return (yVelocity + Mathf.Sqrt((float)yVelocity * (float)yVelocity + 2 * (float)gravity * (ballImpactPosition.y - (float)groundHeight))) / gravity;
    }

    private double CalculateTt(double T12, double zUsedVelocity, Vector3 ballImpactPosition, Vector3 targetPosition)
    {
        return (targetPosition.z - ballImpactPosition.z - zUsedVelocity * T12) / ((1 - dynamicFriction) * zUsedVelocity);
    }

    #endregion

    #region Numeric Solutions

    private double SecantMethod3Order(double H, double I, double J, double K)
    {
        double xn = 0;
        double xnMinus1 = 1;
        double xnMinus2 = 1;
        double error = Calculate3OrderPolynom(xn, H, I, J, K);
        
        while (error > 0.01 || error < -0.01)
        {
            xnMinus2 = xnMinus1;
            xnMinus1 = xn;
            xn = xnMinus1 - Calculate3OrderPolynom(xnMinus1, H, I, J, K) * (xnMinus1 - xnMinus2) / (Calculate3OrderPolynom(xnMinus1, H, I, J, K) - Calculate3OrderPolynom(xnMinus2, H, I, J, K));

            error = Calculate3OrderPolynom(xn, H, I, J, K);
        }

        Debug.Log("xn: " + xn);
        Debug.Log("YVelocity Error: " + error);
        
        return xn;
    }

    #endregion

    private double Calculate3OrderPolynom(double value, double H, double I, double J, double K)
    {
        double error = H * value * value * value + I * value * value + J * value + K;
        //Debug.Log("Error criteria: " + error);
        return error;
    }

    private double CalculateError(double value, double H, double I, double J, double K)
    {
        double error = H * value * value * value + I * value * value + J * value + K;
        Debug.Log("YVelocity Error: " + error);

        return error;
    }
}
