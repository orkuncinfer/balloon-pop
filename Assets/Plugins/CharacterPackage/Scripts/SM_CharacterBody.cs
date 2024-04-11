using UnityEngine;

public class SM_CharacterBody : ActorStateMachine
{
    [SerializeField] private MonoState _grounded;
    [SerializeField] private MonoState _airborne;
    [SerializeField] private MonoState _ability;
    
    [SerializeField] private GameplayTag AbilityStateTag;
    
    protected override MonoState _initialState => _grounded;
    
    TagController _tagController;
    private bool _shouldUseAbility;

    protected override void OnEnter()
    {
        base.OnEnter();
        _tagController = Owner.GetComponent<TagController>();
        _tagController.onGameplaytagAdded += OnGameplayTagAdded;
        _tagController.onGameplaytagRemoved += OnGameplayTagRemoved;
    }

    private void OnGameplayTagRemoved(GameplayTag obj)
    {
        if (obj.Matches(AbilityStateTag))
        {
            _shouldUseAbility = false;
        }
    }

    private void OnGameplayTagAdded(GameplayTag obj)
    {
        Debug.Log("state ability tag added" + obj.FullTag);
        if (obj.Matches(AbilityStateTag))
        {
            Debug.Log("state ability tag matched" + obj.FullTag);
            _shouldUseAbility = true;
        }
        else
        {
            Debug.Log("state ability tag no match" + obj.FullTag);
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