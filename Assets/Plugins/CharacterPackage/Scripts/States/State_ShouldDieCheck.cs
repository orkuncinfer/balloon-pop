using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_ShouldDieCheck : MonoState
{
    private Data_Living _livingData;
    private Data_GAS _gasData;

    protected override void OnEnter()
    {
        base.OnEnter();
        _livingData = Owner.GetData<Data_Living>();
        _gasData = Owner.GetData<Data_GAS>();
        
        _gasData.StatController.GetAttribute("Health").onAttributeChanged += OnHealthChanged;
    }

    private void OnHealthChanged(int arg1, int arg2)
    {
        if (arg2 <= 0)
        {
            _livingData.ShouldDieTrigger = true;
        }
    }
}
