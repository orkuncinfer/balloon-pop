using System.Collections;
using System.Collections.Generic;
using Animancer;
using ECM2;
using UnityEngine;

public class State_PlayLocomotionAsset : MonoState
{
    [SerializeField] private DataGetter<Data_RefVar> _locomotionAsset;
    private Character _character;
    
    private Data_Animancer _dataAnimancer;

    protected override void OnEnter()
    {
        base.OnEnter();
        _character = Owner.GetComponent<Character>();
        _dataAnimancer = Owner.GetData<Data_Animancer>();
        _locomotionAsset.GetData(Owner);
        
        LinearMixerTransitionAsset asset = (LinearMixerTransitionAsset) _locomotionAsset.Data.Value;
        _dataAnimancer.AnimancerComponent.Play(asset);

       
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (_dataAnimancer.AnimancerComponent.States.Current is LinearMixerState linearMixerState)
        {
            linearMixerState.Parameter = _character.GetVelocity().magnitude;
        }
    }
}
