using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_ShouldDieCheck : MonoState
{
    private Data_Living _livingData;
    private Service_GAS _gas;

    protected override void OnEnter()
    {
        base.OnEnter();
        _livingData = Owner.GetData<Data_Living>();
        _gas = Owner.GetService<Service_GAS>();
        
        _gas.StatController.GetAttribute("Health").onAttributeChanged += OnHealthChanged;
    }

    private void OnHealthChanged(int arg1, int arg2)
    {
        if (arg2 <= 0)
        {
            _livingData.ShouldDieTrigger = true;
        }
    }
}
