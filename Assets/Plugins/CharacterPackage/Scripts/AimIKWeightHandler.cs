using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class AimIKWeightHandler : MonoBehaviour
{
    public bool IsAiming;
    public float LerpSpeed = 10f;
    private AimIK _aimIK;

    private void Start()
    {
        _aimIK = GetComponent<AimIK>();
    }

    private void Update()
    {
        //lerp weight
        if (IsAiming)
        {
            _aimIK.solver.IKPositionWeight = Mathf.Lerp(_aimIK.solver.IKPositionWeight, 1, Time.deltaTime * LerpSpeed);
        }
        else
        {
            _aimIK.solver.IKPositionWeight = Mathf.Lerp(_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * LerpSpeed);
        }
    }
}