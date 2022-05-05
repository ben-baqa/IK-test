using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Run", menuName = "ScriptableObjects/Run Settings", order = 1)]
public class RunSettings : ScriptableObject
{
    [Header("Steps")]
    public StepCurves stepCurves;
    public float stepDist, footOffset,
        stepTime, settleTime,
        sideStepMod, stepHeightMod;

    private void Step(Transform foot, Vector3 pos)
    {

    }
}
