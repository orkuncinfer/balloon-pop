using System;
using FishNet.Object;
using StatSystem;
using UnityEngine;

public class Network_ApplyEquipmentModifiers : MonoBehaviour
{
    public ItemListDefinition _allEffects;
    private Equippable _equippable;

    private NetworkObject _networkObject;

    private bool isServer => _networkObject.IsServerInitialized;
    private void Start()
    {
        _equippable = GetComponent<Equippable>();

        if (_equippable.IsEquipped)
        {
            OnEquipped(_equippable.Owner);
        }
        else
        {
            _equippable.onEquipped += OnEquipped;
        }

        _equippable.onUnequipped += OnUnequipped;
    }

    private void OnUnequipped(ActorBase obj)
    {
        _networkObject = obj.GetComponent<NetworkObject>();
        if(!isServer) return;
        
        StatController statController = obj.GetService<Service_GAS>().StatController;

        foreach (var itemDataAttribute in _equippable.ItemData.Attributes)
        {
            GameplayEffectDefinition itemAttributeEffectDefinition = _allEffects.GetItem(itemDataAttribute.Key) as GameplayEffectDefinition;
            
            statController.GetStat(itemAttributeEffectDefinition.ModifierDefinitions[0].StatName).RemoveModifierFromSource(this);
            Debug.Log($"modifier removed from stat : {itemAttributeEffectDefinition.ModifierDefinitions[0].StatName} by {transform.name}");
        }
    }

    private void OnEquipped(ActorBase obj)
    {
        _networkObject = obj.GetComponent<NetworkObject>();
        if(!isServer) return;
        
        Debug.Log($"modifier added : equipped");
        StatController statController = obj.GetService<Service_GAS>().StatController;

        foreach (var itemDataAttribute in _equippable.ItemData.Attributes)
        {
            GameplayEffectDefinition itemAttributeEffectDefinition = _allEffects.GetItem(itemDataAttribute.Key) as GameplayEffectDefinition;

            StatModifier newModifier = new StatModifier();
            newModifier.Type = itemAttributeEffectDefinition.ModifierDefinitions[0].Type;
            newModifier.Magnitude = float.Parse(itemDataAttribute.Value);
            newModifier.Source = this;
            
            statController.GetStat(itemAttributeEffectDefinition.ModifierDefinitions[0].StatName).AddModifier(newModifier);
            Debug.Log($"modifier added : {newModifier.Type} , {newModifier.Magnitude}");
        }
    }
}
