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

    private InteractionTrigger _lastInteractionTrigger;
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

    protected override void OnUpdate()
    {
        base.OnUpdate();
        int closestTriggerIndex = _interactionSystem.GetClosestTriggerIndex();

        // ...if none found, do nothing
        if (closestTriggerIndex == -1)
        {
            RemoveLastInteractable();
            return;
        }

        // ...if the effectors associated with the trigger are in interaction, do nothing
        if (!_interactionSystem.TriggerEffectorsReady(closestTriggerIndex))
        {
            RemoveLastInteractable();
            return;
        }

        if (_lastInteractionTrigger != _interactionSystem.triggersInRange[closestTriggerIndex])
        {
            _lastInteractionTrigger = _interactionSystem.triggersInRange[closestTriggerIndex];
            if(_lastInteractionTrigger.TryGetComponent<Interactable>(out var interactionDisplayer))
            {
                interactionDisplayer.SetInteractable(true);
            }
        }
    }

    private void OnPerformed(InputAction.CallbackContext obj)
    {
        if(_lastInteractionTrigger == null) return;
        if(_lastInteractionTrigger.TryGetComponent<Interactable>(out var interactable))
        {
            interactable.Interact(Owner);
        }
    }

    private void RemoveLastInteractable()
    {
        if (_lastInteractionTrigger != null)
        {
            if(_lastInteractionTrigger.TryGetComponent<Interactable>(out var interactionDisplayer))
            {
                interactionDisplayer.SetInteractable(false);
            }
            _lastInteractionTrigger = null;
        }
    }
}
