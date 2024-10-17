using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


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
    public event Action<ActorBase> onEquipped; 
    public string EquipSocketName => _equipSocketName;
    
    public virtual void OnEquip(ActorBase owner)
    {
        _owner = owner;
        Owner.GetData<DS_EquipmentUser>().SocketName = _equipSocketName;
        if (_overrideLocomotionAsset != null)
        {
            _owner.GetData<Data_RefVar>("Locomotion").Value = _overrideLocomotionAsset;
        }
        onEquipped?.Invoke(owner);
    }
    [Button]
    public virtual void EquipThisInstance(ActorBase actor)
    {
        Owner = actor;
        Owner.GetData<DS_EquipmentUser>().EquipWorldInstance(gameObject,EquipSocketName);
    }
}
