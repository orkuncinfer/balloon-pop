using System.Collections;
using System.Collections.Generic;
using Animancer;
using ECM2;
using UnityEngine;

public class State_PlayLocomotionAsset : MonoState
{
    [SerializeField] private LinearMixerTransitionAsset Asset;
    private Character _character;
    
    private Data_Animancer _dataAnimancer;

    protected override void OnEnter()
    {
        base.OnEnter();
        _character = Owner.GetComponent<Character>();
        _dataAnimancer = Owner.GetData<Data_Animancer>();
        
        _dataAnimancer.AnimancerComponent.Play(Asset);

       
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
