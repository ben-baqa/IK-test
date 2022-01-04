using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// provides a neat little abstraction for procedural steps
// with precise custom settings
public class Step
{
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public Vector3 direction, sideDirection;
    public StepCurves curves;

    public Step(Vector3 start, Vector3 target,
        StepCurves settings, float flip = 1)
    {
        startPosition = start;
        targetPosition = target;
        direction = target - start;
        sideDirection = Vector3.Cross(direction, Vector3.up);
        sideDirection = sideDirection.normalized * flip;
        curves = settings;
    }

    public Vector3 GetPosition(float val)
    {
        return startPosition + curves.Solve(
            direction, sideDirection, val);
    }
}

[System.Serializable]
public class StepCurves
{
    public AnimationCurve forwardCurve;
    public WeightedCurve verticalCurve;
    public WeightedCurve sideCurve;

    public Vector3 Solve(Vector3 forward, Vector3 side, float val)
    {
        return forward * forwardCurve.Evaluate(val)
            + side * sideCurve.Evaluate(val)
            + Vector3.up * verticalCurve.Evaluate(val);
    }
}

[System.Serializable]
public class WeightedCurve
{
    public float amplitude;
    public AnimationCurve curve;

    public float Evaluate(float val)
    {
        return amplitude * curve.Evaluate(val);
    }
}