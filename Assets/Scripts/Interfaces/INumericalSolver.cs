using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INumericalSolver
{
    float Solve(IMathFunction f);
}
