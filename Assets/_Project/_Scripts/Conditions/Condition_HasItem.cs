using StatSystem;
using UnityEngine;
public class Condition_HasItem : GameCondition
{
    [SerializeField] private ItemDefinition[] _items;
    public override bool IsConditionMet(ActorBase actor)
    {
        bool hasAll = true;
        
        for (int i = 0; i < _items.Length; i++)
        {
            if (!DefaultPlayerInventory.Instance.HasItem(_items[i].ItemId))
            {
                hasAll = false;
                break;
            }
        }

        return hasAll;
    }
}
