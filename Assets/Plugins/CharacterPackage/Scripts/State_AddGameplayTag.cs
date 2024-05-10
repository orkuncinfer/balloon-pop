using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_AddGameplayTag : MonoState
{
    [SerializeField] private List<GameplayTag> _tagsToAdd;
    [SerializeField] private bool _removeOnExit;
    private Data_GAS _gasData;
    
    protected override void OnEnter()
    {
        base.OnEnter();
        _gasData = Owner.GetData<Data_GAS>();
        foreach (var tag in _tagsToAdd)
        {
            _gasData.TagController.AddTag(tag);
        }
    }

    protected override void OnExit()
    {
        base.OnExit();
        if (_removeOnExit)
        {
            foreach (var tag in _tagsToAdd)
            {
                _gasData.TagController.RemoveTag(tag);
            }
        }
    }
}
