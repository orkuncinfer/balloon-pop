using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
[System.Serializable]
public class DS_EquipmentUser : Data
{
    [SerializeField][HideInPlayMode]
    private GameObject _equipmentPrefab;
    public event Action<GameObject,GameObject> onEquipmentPrefabChanged;
    [ShowInInspector][HideInEditorMode]
    public GameObject EquipmentPrefab
    {
        get => _equipmentPrefab;
        set
        {
            GameObject oldValue = _equipmentPrefab;
            bool isChanged = _equipmentPrefab != value;
            _equipmentPrefab = value;
            if (isChanged)
            {
                onEquipmentPrefabChanged?.Invoke(oldValue,value);
                EquipmentPrefabChanged(oldValue,value);
            }
        }
    }
    
    public event Action<GameObject> onEquipmentInstanceChanged;
    [SerializeField][ReadOnly] private GameObject _equipmentInstance;
    public GameObject EquipmentInstance
    {
        get => _equipmentInstance;
        set
        {
            if (_equipmentInstance != value)
            {
                _equipmentInstance = value;
                onEquipmentInstanceChanged?.Invoke(_equipmentInstance);
            }
        }
    }

    [SerializeField] private string _socketName;

    public string SocketName
    {
        get => _socketName;
        set => _socketName = value;
    }

    private bool _equippedAsWorldInstance;

    public override void OnActorStarted()
    {
        base.OnActorStarted();
        EquipCurrent();
    }

    private void EquipmentPrefabChanged(GameObject oldPrefab, GameObject newPrefab)
    {
        if (_equippedAsWorldInstance)
        {
            _equippedAsWorldInstance = false;
            return;
        }
        ReleaseInstance();
        EquipCurrent();
    }
    
    
    public void EquipCurrent()
    {
        ReleaseInstance();
        EquipmentInstance = PoolManager.SpawnObject(EquipmentPrefab, Vector3.zero, Quaternion.identity);
        
        if (EquipmentInstance.TryGetComponent(out Weapon weapon))
        {
            weapon.Owner = OwnerActor;
            weapon.OnEquip(OwnerActor);
        }
        
        EquipmentInstance.transform.SetParent(OwnerActor.GetSocket(SocketName));
        EquipmentInstance.transform.localPosition = Vector3.zero;
        EquipmentInstance.transform.localEulerAngles = Vector3.zero;
    }
    
    public void EquipWorldInstance(GameObject equipmentInstance,string socketName = "")
    {
        _equippedAsWorldInstance = true;
        ReleaseInstance();
        
        EquipmentPrefab = equipmentInstance;
        EquipmentInstance = equipmentInstance;
        if(socketName != "") SocketName = socketName;
        
        if (EquipmentInstance.TryGetComponent(out Weapon weapon))
        {
            weapon.Owner = OwnerActor;
            weapon.OnEquip(OwnerActor);
        }
        
        EquipmentInstance.transform.SetParent(OwnerActor.GetSocket(SocketName));
        EquipmentInstance.transform.localPosition = Vector3.zero;
        EquipmentInstance.transform.localEulerAngles = Vector3.zero;
    }

    private void ReleaseInstance()
    {
        if (EquipmentInstance != null)
        {
            PoolManager.ReleaseObject(EquipmentInstance);
            EquipmentInstance = null;
        }
    }
}
