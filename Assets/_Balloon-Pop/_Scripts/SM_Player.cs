using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_Player : ActorStateMachine
{
    [SerializeField] private MonoState _alive;
    [SerializeField] private MonoState _dead;
    
    protected override MonoState _initialState => _alive;
    
    private DS_PlayerRuntime _playerRuntime;
    public override void OnInitialize()
    {
        base.OnInitialize();
        _playerRuntime = Owner.GetData<DS_PlayerRuntime>();
    }

    public override void OnRequireAddTransitions()
    {
        if(_playerRuntime.CurrentHealth <= 0)
        {
            Debug.Log("Player is dead");
        }
        AddTransition(_alive, _dead, () => _playerRuntime.CurrentHealth <= 0);
    }
}
