﻿using System;
using Animancer;
using BandoWare.GameplayTags;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


public class Equippable : MonoBehaviour
{
    [SerializeField][ReadOnly]
    private ActorBase _owner; 
    public ActorBase Owner 
    {
        get => _owner;
        set => _owner = value;
    }

    public ItemDefinition ItemDefinition;
    
    [SerializeField]
    private ScriptableObject _overrideLocomotionAsset;
    
    [SerializeField] private string _equipSocketName;
    
    [SerializeField] private AbilityDefinition _equipAbility;
    [SerializeField] private AbilityDefinition _unequipAbility;
    [SerializeField] private AbilityDefinition[] _grantedAbilities;
    public event Action<ActorBase> onEquipped; 
    public event Action<ActorBase> onUnequipped; 
    public string EquipSocketName => _equipSocketName;
    
    public GameplayTagContainer EquipTags;
    
    private Object _previousLocomotionAsset;

    public bool IsEquipped => _isEquipped;
    private bool _isEquipped;
    
    public virtual void OnEquip(ActorBase owner)
    {
        _owner = owner;
        Owner.GetData<DS_EquipmentUser>().SocketName = _equipSocketName;
        if (_overrideLocomotionAsset != null)
        {
            _previousLocomotionAsset = owner.GetData<Data_RefVar>("Locomotion").Value;
            _owner.GetData<Data_RefVar>("Locomotion").Value = _overrideLocomotionAsset;
        }
        
        _isEquipped = true;
        Owner.GameplayTags.AddTags(EquipTags);
        foreach (var abilityDefinition in _grantedAbilities)
        {
            Owner.GetData<Data_GAS>().AbilityController.AddAbilityIfNotHave(abilityDefinition);
        }
        onEquipped?.Invoke(owner);
    }

    public virtual void OnUnequip(ActorBase actor)
    {
        if (_overrideLocomotionAsset != null)
        {
            _owner.GetData<Data_RefVar>("Locomotion").Value = _previousLocomotionAsset;
        }
        _isEquipped = false;
        Owner.GameplayTags.RemoveTags(EquipTags);
        foreach (var abilityDefinition in _grantedAbilities)
        {
            Owner.GetData<Data_GAS>().AbilityController.RemoveAbilityIfHave(abilityDefinition);
        }
        onUnequipped?.Invoke(Owner);
    }
    
    [Button]
    public virtual void EquipThisInstance(ActorBase actor)
    {
        Owner = actor;
        Owner.GetData<DS_EquipmentUser>().EquipWorldInstance(gameObject,EquipSocketName);
    }
    [Button]
    public ActiveAbility TryUnequipWithAbility()
    {
        return Owner.GetData<Data_GAS>().AbilityController.AddAndTryActivateAbility(_unequipAbility);
    }
    [Button]
    public ActiveAbility TryEquipWithAbility()
    {
        return Owner.GetData<Data_GAS>().AbilityController.AddAndTryActivateAbility(_equipAbility);
    }
}