using BandoWare.GameplayTags;
using UnityEngine;

public class SM_CharacterBody : ActorStateMachine
{
    [SerializeField] private MonoState _grounded;
    [SerializeField] private MonoState _airborne;
    [SerializeField] private MonoState _ability;
    
    [SerializeField] private BandoWare.GameplayTags.GameplayTag AbilityStateTag;
    
    protected override MonoState _initialState => _grounded;
    
    TagController _tagController;
    private Data_GAS _gasData;
    private bool _shouldUseAbility;

    protected override void OnEnter()
    {
        base.OnEnter();
        _gasData = Owner.GetData<Data_GAS>();
        _tagController = _gasData.TagController;
        Owner.GameplayTags.OnTagChanged += OnAnyTagCountChange;
    }

    protected override void OnExit()
    {
        base.OnExit();
        Owner.GameplayTags.OnTagChanged -= OnAnyTagCountChange;
    }

    private void OnAnyTagCountChange()
    {
        if(AbilityStateTag == BandoWare.GameplayTags.GameplayTag.None) return;
        if (Owner.GameplayTags.HasTagExact(AbilityStateTag))
        {
            _shouldUseAbility = true;
        }else
        {
            _shouldUseAbility = false; // no match
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