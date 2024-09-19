using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] private string m_Id;
    public string id => m_Id;
    
    [SerializeField]
    private ActorBase _owner; 
    public ActorBase Owner 
    {
        get => _owner;
        set => _owner = value;
    }

    [SerializeField]
    private ScriptableObject _overrideLocomotionAsset;
    
    [SerializeField] private string _equipSocketName;
    public string EquipSocketName => _equipSocketName;
    
    public virtual void OnEquip(ActorBase owner)
    {
        _owner = owner;
        Owner.GetData<DS_EquipmentUser>().SocketName = _equipSocketName;
        if (_overrideLocomotionAsset != null)
        {
            _owner.GetData<Data_RefVar>("Locomotion").Value = _overrideLocomotionAsset;
        }
    }
    [Button]
    public virtual void EquipThisInstance(ActorBase actor)
    {
        Owner = actor;
        Owner.GetData<DS_EquipmentUser>().EquipWorldInstance(gameObject,EquipSocketName);
    }

    private void Reset()
    {
        m_Id = gameObject.name;
    }
}
