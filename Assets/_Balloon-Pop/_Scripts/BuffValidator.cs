using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuffValidator 
{
    public static bool HasPierceBuff()
    {
        return true;
    }
    public static bool HasMagnetBuff()
    {
        return true;
    }
    public static bool Has3ShotBuff()
    {
        return true;
    }
    public static bool HasRicochet2Buff()
    {
        return true;
    }
    public static bool HasRicochet4Buff()
    {
        return true;
    }
    
    public static bool HasBuff(ItemDefinition itemDefinition)
    {
        bool hasInPersistent = GlobalData.GetData<DS_PlayerPersistent>().Inventory.ContainsKey(itemDefinition.ItemId);
        bool hasInRuntime = ActorRegistry.PlayerActor.GetData<DS_PlayerPersistent>().RuntimeBuffs.ContainsKey(itemDefinition.ItemId);
        return hasInPersistent || hasInRuntime;
    }
}
