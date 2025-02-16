using Sirenix.Utilities;
using UnityEngine;

public class UI_EquipmentHudControl : MonoBehaviour
{
    [SerializeField] private Sherbert.Framework.Generic.SerializableDictionary<int,UI_ItemElement> _equipmentSlots;
    [SerializeField] private InventoryDefinition _equipmentInventory;
    private ActorBase _owner;
    

    private void Awake()
    {
        _owner = ActorUtilities.FindFirstActorInParents(transform);
        _equipmentInventory.onInventoryChanged += OnInventoryChanged;
    }

    private void OnDestroy()
    {
        _equipmentInventory.onInventoryChanged -= OnInventoryChanged;
    }

    private void Start()
    {
        //OnInventoryChanged();
    }

    private void OnInventoryChanged()
    {
       /* for (int i = 0; i < _equipmentInventory.InventoryData.InventorySlots.Count; i++)
        {
            if (_equipmentInventory.InventoryData.InventorySlots[i].ItemID.IsNullOrWhitespace())
            {
                _equipmentSlots[i].ClearItemData();
                continue;
            }
            _equipmentSlots[i].SetItemData(_equipmentInventory.InventoryData.InventorySlots[i].ItemID, _equipmentInventory.InventoryData.InventorySlots[i]);
        }*/
       foreach (var slot in _equipmentSlots)
       {
              if (_equipmentInventory.InventoryData.InventorySlots[slot.Key].ItemID.IsNullOrWhitespace())
              {
                slot.Value.ClearItemData();
                continue;
              }
              slot.Value.SetItemData(_equipmentInventory.InventoryData.InventorySlots[slot.Key].ItemID, _equipmentInventory.InventoryData.InventorySlots[slot.Key]);
       }
    }
}
