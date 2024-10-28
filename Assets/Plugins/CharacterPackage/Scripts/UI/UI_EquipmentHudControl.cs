using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class UI_EquipmentHudControl : MonoBehaviour
{
    [SerializeField] private InventoryDefinition _equipmentInventory;
    private ActorBase _owner;
    
    [SerializeField] private UI_ItemElement[] _equipmentSlots;

    private void Awake()
    {
        _owner = ActorUtilities.FindFirstActorInParents(transform);
        _equipmentInventory.onInventoryChanged += OnInventoryChanged;
    }

    private void Start()
    {
        //OnInventoryChanged();
    }

    private void OnInventoryChanged()
    {
        for (int i = 0; i < _equipmentInventory.InventoryData.InventorySlots.Count; i++)
        {
            if (_equipmentInventory.InventoryData.InventorySlots[i].ItemID.IsNullOrWhitespace())
            {
                _equipmentSlots[i].ClearItemData();
                continue;
            }
            _equipmentSlots[i].SetItemData(_equipmentInventory.InventoryData.InventorySlots[i].ItemID, _equipmentInventory.InventoryData.InventorySlots[i]);
        }
    }
}
