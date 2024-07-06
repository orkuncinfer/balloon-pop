using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItemCount : MonoCore
{
    [SerializeField] private ItemDefinition _itemDefinition;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _icon;
    
    private void OnEnable()
    {
        
    }

    protected override void OnReady()
    {
        base.OnReady();
        DefaultPlayerInventory.Instance.onItemChanged += OnItemChanged;
        _icon.sprite = _itemDefinition.Icon;
        _text.text = DefaultPlayerInventory.Instance.GetItemCount(_itemDefinition.ItemId).ToString();
    }


    private void OnItemChanged(string itemId, int oldAmount, int newAmount)
    {
        if(itemId != _itemDefinition.ItemId) return;
        _text.text = DefaultPlayerInventory.Instance.GetItemCount(_itemDefinition.ItemId).ToString();
    }
}
