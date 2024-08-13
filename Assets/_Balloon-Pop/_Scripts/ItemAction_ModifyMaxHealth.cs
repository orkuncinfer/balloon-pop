using System.Collections;
using System.Collections.Generic;
using StatSystem;
using UnityEngine;
using UnityEngine.Pool;

public class ItemAction_ModifyMaxHealth : SimpleAction
{
    public int ModifierValue;
    public ModifierOperationType ModifierOperation;
    public bool CurrentHealthToMax;

    public override void OnAction(ActorBase owner)
    {
        base.OnAction(owner);
        DS_PlayerPersistent playerData = GlobalData.GetData<DS_PlayerPersistent>();
        if (ModifierOperation == ModifierOperationType.Additive)
        {
            ApplyAdditive(playerData);
        }
        DS_PlayerRuntime playerRuntime = owner.GetData<DS_PlayerRuntime>();
        if(CurrentHealthToMax)
        {
            playerRuntime.CurrentHealth = playerData.MaxHealth;
        }
        playerData.SaveData();
    }

    void ApplyAdditive(DS_PlayerPersistent playerData)
    {
        playerData.MaxHealth += ModifierValue;
    }
}
