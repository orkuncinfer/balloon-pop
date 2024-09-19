using System;
using System.Collections;
using System.Collections.Generic;
using ECM2;
using StatSystem;
using UnityEngine;

public class BindMoveSpeedToStat : MonoBehaviour
{
    [SerializeField] private StatController _statController;
    [SerializeField] private Character _character;

    private void Awake()
    {
        _statController.onInitialized += OnStatControllerInitialized;
    }

    private void OnStatControllerInitialized()
    {
        _character.maxWalkSpeed = _statController.Stats["MoveSpeed"].Value;
        _statController.onStatIsModified += OnStatIsModified;
    }

    private void OnStatIsModified(Stat stat)
    {
        if (stat.Definition.name == "MoveSpeed")
        {
            _character.maxWalkSpeed = stat.Value;
        }
    }
}
