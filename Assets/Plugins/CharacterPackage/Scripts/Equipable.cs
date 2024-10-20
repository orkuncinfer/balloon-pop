using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


public class Equipable : MonoBehaviour
{
    [SerializeField][ReadOnly]
    private ActorBase _owner; 
    public ActorBase Owner 
    {
        get => _owner;
        set => _owner = value;
    }

    [SerializeField]
    private ScriptableObject _overrideLocomotionAsset;
    
    [SerializeField] private string _equipSocketName;
    
    [SerializeField] private AbilityDefinition _equipAbility;
    [SerializeField] private AbilityDefinition _unequipAbility;
    public event Action<ActorBase> onEquipped; 
    public event Action<ActorBase> onUnequipped; 
    public string EquipSocketName => _equipSocketName;
    
    private Object _previousLocomotionAsset;
    
    public virtual void OnEquip(ActorBase owner)
    {
        _owner = owner;
        Owner.GetData<DS_EquipmentUser>().SocketName = _equipSocketName;
        if (_overrideLocomotionAsset != null)
        {
            _previousLocomotionAsset = owner.GetData<Data_RefVar>("Locomotion").Value;
            _owner.GetData<Data_RefVar>("Locomotion").Value = _overrideLocomotionAsset;
        }
        onEquipped?.Invoke(owner);
    }

    public virtual void OnUnequip(ActorBase actor)
    {
        if (_overrideLocomotionAsset != null)
        {
            _owner.GetData<Data_RefVar>("Locomotion").Value = _previousLocomotionAsset;
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
    public void TryUnequipWithAbility()
    {
        Owner.GetData<Data_GAS>().AbilityController.TryActiveAbilityWithDefinition(_unequipAbility);
    }
    [Button]
    public void TryEquipWithAbility()
    {
        Owner.GetData<Data_GAS>().AbilityController.TryActiveAbilityWithDefinition(_equipAbility);
    }
}
