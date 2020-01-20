using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequantMethod : INumericalSolver
{
    private float xn;
    private float xnMinus1;
    private float xnMinus2;

    public SequantMethod(float xn, float xnMinus1)
    {
        this.xn = xn;
        this.xnMinus1 = xnMinus1;
    }

    public float Solve(IMathFunction f)
    {
        float currentFunctionEvaluation = f.Evaluate(xn);

        while ( currentFunctionEvaluation > 0.01 || currentFunctionEvaluation < -0.01)
        {
            xnMinus2 = xnMinus1;
            xnMinus1 = xn;
            xn = xnMinus1 - currentFunctionEvaluation * (xnMinus1 - xnMinus2) / (currentFunctionEvaluation - f.Evaluate(xnMinus2));
        }

        return xn;
    }
}
