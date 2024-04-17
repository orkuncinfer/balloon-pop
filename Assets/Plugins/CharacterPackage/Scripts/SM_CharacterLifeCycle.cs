using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_CharacterLifeCycle : ActorStateMachine
{
    [SerializeField] private MonoState _alive;
    [SerializeField] private MonoState _dead;
    [SerializeField] private MonoState _revive;
    
    [SerializeField] private GameplayTag AbilityStateTag;
    [SerializeField] private DataGetter<Data_Player> _dataPlayer;
    
    protected override MonoState _initialState => _alive;

    protected override void OnEnter()
    {
        base.OnEnter();
        _dataPlayer.GetData();
    }

    public override void OnRequireAddTransitions()
    {
        
    }

    private bool AbilityToAlive()
    {
        return !_dataPlayer.Data.TagController.Matches(AbilityStateTag);
    }


    private bool AliveToAbility()
    {
        return _dataPlayer.Data.TagController.Matches(AbilityStateTag);
    }
}
