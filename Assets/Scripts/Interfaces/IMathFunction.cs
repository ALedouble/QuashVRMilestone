using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMathFunction
{
    void SetValues(float[] values);
    float Evaluate(float x);
}
