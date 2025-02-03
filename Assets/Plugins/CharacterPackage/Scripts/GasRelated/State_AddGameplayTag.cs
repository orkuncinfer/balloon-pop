using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_AddGameplayTag : MonoState
{
    [SerializeField] private GameplayTagContainer _tagsToAdd;
    [SerializeField] private bool _removeOnExit;
    private Data_GAS _gasData;
    
    protected override void OnEnter()
    {
        base.OnEnter();
        _gasData = Owner.GetData<Data_GAS>();
        Owner.GameplayTags.AddTags(_tagsToAdd);
    }

    protected override void OnExit()
    {
        base.OnExit();
        if (_removeOnExit)
        {
           Owner.GameplayTags.RemoveTags(_tagsToAdd);
        }
    }
}
