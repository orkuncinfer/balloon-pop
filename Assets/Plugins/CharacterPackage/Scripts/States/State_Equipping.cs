using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Equipping : MonoState
{
    [SerializeField] private AbilityDefinition _unequipAbility;
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Owner.GetData<Data_GAS>().AbilityController.AddAndTryActivateAbility(_unequipAbility);
        }
    }
}
