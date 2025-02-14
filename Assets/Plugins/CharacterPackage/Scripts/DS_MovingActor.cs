using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DS_MovingActor : Data
{
    [SerializeField] private Vector3 _lookdirection;
    public Vector3 LookDirection
    {
        get => _lookdirection;
        set => _lookdirection = value;
    }
}
