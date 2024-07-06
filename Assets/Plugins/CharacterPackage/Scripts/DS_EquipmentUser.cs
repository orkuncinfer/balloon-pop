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
            }
        }
    }
    
    public event Action<GameObject> onEquipmentInstanceChanged;
    [SerializeField] private GameObject _equipmentInstance;
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
    public string SocketName => _socketName;
}
