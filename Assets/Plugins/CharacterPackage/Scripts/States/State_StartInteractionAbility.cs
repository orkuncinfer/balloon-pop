using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.InputSystem;

public class State_StartInteractionAbility : MonoState
{
    [SerializeField] private DSGetter<Data_AbilityDefinition> _abilityDS;
    [SerializeField] private Data_GAS _gasData;
    public InputActionAsset ActionAsset;
    [SerializeField] private AbilityDefinition _interactionAbility;
    public string ActionName;
    
    private InputAction _abilityAction;
    private InteractionSystem _interactionSystem;
    protected override void OnEnter()
    {
        base.OnEnter();
        _gasData = Owner.GetData<Data_GAS>();
        _interactionSystem = Owner.GetComponentInChildren<InteractionSystem>();
        _abilityDS.GetData(Owner);
        
        _abilityAction = ActionAsset.FindAction(ActionName);
        _abilityAction.performed += OnPerformed;
        
        _abilityAction?.Enable();
    }

    protected override void OnExit()
    {
        base.OnExit();
        _abilityAction.performed -= OnPerformed;
    }

    private void OnPerformed(InputAction.CallbackContext obj)
    {
        int closestTriggerIndex = _interactionSystem.GetClosestTriggerIndex();

        // ...if none found, do nothing
        if (closestTriggerIndex == -1) return;

        // ...if the effectors associated with the trigger are in interaction, do nothing
        if (!_interactionSystem.TriggerEffectorsReady(closestTriggerIndex)) return;
        
        _gasData.AbilityController.AddAndTryActivateAbility(_interactionAbility);
    }
}
