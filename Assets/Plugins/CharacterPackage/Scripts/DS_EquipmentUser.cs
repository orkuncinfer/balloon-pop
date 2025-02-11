using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
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

    [SerializeField] private GameObject _itemToEquip;
    public GameObject ItemToEquip
    {
        get => _itemToEquip;
        set => _itemToEquip = value;
    }
    
    [SerializeField] private string _socketName;

    public string SocketName
    {
        get => _socketName;
        set => _socketName = value;
    }

    public float LerpSpeed;

    private bool _equippedAsWorldInstance;//??

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
        if(EquipmentPrefab == null) return;
        ReleaseInstance();
        if (_equipmentPrefab.gameObject.activeInHierarchy) // if exist in world
        {
            EquipmentInstance = EquipmentPrefab;
        }
        else
        {
            EquipmentInstance = PoolManager.SpawnObject(EquipmentPrefab, Vector3.zero, Quaternion.identity);
        }
        
        if (EquipmentInstance.TryGetComponent(out Equippable weapon))
        {
            weapon.Owner = OwnerActor;
            weapon.OnEquip(OwnerActor);
        }
        
        
        SetInstanceTransform();
    }
    public void UnequipCurrent(bool destroyInstance = true)
    {
        if (_equipmentInstance != null)
        {
            if (EquipmentInstance.TryGetComponent(out Equippable weapon))
            {
                weapon.OnUnequip(OwnerActor);
            }
            EquipmentInstance = null;
            if(destroyInstance)
                ReleaseInstance();
        }
    }
    public void EquipWorldInstance(GameObject equipmentInstance,string socketName = "")
    {
        _equippedAsWorldInstance = true;
        ReleaseInstance();
        
        EquipmentPrefab = equipmentInstance;
        EquipmentInstance = equipmentInstance;
        if(socketName != "") SocketName = socketName;
        
        if (EquipmentInstance.TryGetComponent(out Equippable weapon))
        {
            weapon.Owner = OwnerActor;
            weapon.OnEquip(OwnerActor);
        }
        
        SetInstanceTransform();
        equipmentInstance.SetActive(true);
    
    }
    
    private void SetInstanceTransform()
    {
        if (EquipmentInstance == null) return;
        EquipmentInstance.transform.SetParent(OwnerActor.GetSocket(SocketName),true);
        if(LerpSpeed > 0)
        {
            EquipmentInstance.transform.DOLocalRotate(Vector3.zero, LerpSpeed);
            EquipmentInstance.transform.DOLocalMove(Vector3.zero, LerpSpeed).OnComplete(() =>
            {
                EquipmentInstance.transform.SetParent(OwnerActor.GetSocket(SocketName));
                EquipmentInstance.transform.localPosition = Vector3.zero;
                EquipmentInstance.transform.localEulerAngles = Vector3.zero;
                EquipmentInstance.transform.localScale = Vector3.one;
                LerpSpeed = 0;
                Debug.Log("Equipped equipment instance");
            });
            return;
        }
        
        EquipmentInstance.transform.localPosition = Vector3.zero;
        EquipmentInstance.transform.localEulerAngles = Vector3.zero;
        EquipmentInstance.transform.localScale = Vector3.one;
        
        Debug.Log("Equipped equipment instance");
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
