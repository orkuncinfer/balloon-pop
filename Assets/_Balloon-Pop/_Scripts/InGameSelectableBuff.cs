using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InGameSelectableBuff : MonoBehaviour
{
    public ItemDefinition BuffItem;

    [SerializeField] private Image _buffImage;
    [SerializeField] private TextMeshProUGUI _buffDescription;
    [SerializeField] private TextMeshProUGUI _buffTitle;
    [SerializeField] private Button _selectButton;
    
    public Action<string> onBuffSelected;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(OnSelected);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(OnSelected);
    }

    public void SetItem(ItemDefinition item)
    {
        BuffItem = item;
        _buffImage.sprite = BuffItem.Icon;
        _buffDescription.text = BuffItem.Description;
        _buffTitle.text = BuffItem.ItemName;
      
    }

    public void OnSelected()
    {
        onBuffSelected?.Invoke(BuffItem.ItemId);
       
        DS_PlayerPersistent playerPersistent = GlobalData.GetData<DS_PlayerPersistent>();
        int currentCount = playerPersistent.RuntimeBuffs.ContainsKey(BuffItem.ItemId) ? playerPersistent.RuntimeBuffs[BuffItem.ItemId] : 0;
        playerPersistent.RuntimeBuffs[BuffItem.ItemId] = currentCount + 1;
        foreach (var itemAction in BuffItem.ItemActions)
        {
            itemAction.OnAction(ActorRegistry.PlayerActor);
        }
        Debug.Log("BuffSelecteeed");
    }
}