using UnityEngine;

public class SM_CharacterBody : ActorStateMachine
{
    [SerializeField] private MonoState _grounded;
    [SerializeField] private MonoState _airborne;
    [SerializeField] private MonoState _ability;
    
    [SerializeField] private GameplayTag AbilityStateTag;
    
    protected override MonoState _initialState => _grounded;
    
    TagController _tagController;
    private Data_GAS _gasData;
    private bool _shouldUseAbility;

    protected override void OnEnter()
    {
        base.OnEnter();
        _gasData = Owner.GetData<Data_GAS>();
        _tagController = _gasData.TagController;
        _tagController.onGameplaytagAdded += OnGameplayTagAdded;
        _tagController.onGameplaytagRemoved += OnGameplayTagRemoved;
    }

    private void OnGameplayTagRemoved(GameplayTag obj)
    {
        if (obj.MatchesExact(AbilityStateTag))
        {
            _shouldUseAbility = false;
        }
    }

    private void OnGameplayTagAdded(GameplayTag obj)
    {
        if (obj.Matches(AbilityStateTag))
        {
            _shouldUseAbility = true;
        }
        else
        {
            // no match
        }
    }

    public override void OnRequireAddTransitions()
    {
        AddTransition(_grounded,_ability, GroundedToAbility);
        AddTransition(_ability,_grounded, AbilityToGrounded);
    }

    private bool AbilityToGrounded()
    {
        return !_shouldUseAbility;
    }

    private bool GroundedToAbility()
    {
        return _shouldUseAbility;
    }
}