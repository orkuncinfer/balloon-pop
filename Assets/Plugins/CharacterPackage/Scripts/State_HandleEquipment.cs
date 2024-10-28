using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_HandleEquipment : MonoState
{
    private DS_EquipmentUser _equipmentData;
    protected override void OnEnter()
    {
        base.OnEnter();
        _equipmentData = Owner.GetData<DS_EquipmentUser>();
        
        
        _equipmentData.onEquipmentPrefabChanged += OnEquipmentPrefabChanged;
    }

    protected override void OnExit()
    {
        base.OnExit();
        _equipmentData.onEquipmentPrefabChanged -= OnEquipmentPrefabChanged;
    }

    private void OnEquipmentPrefabChanged(GameObject arg1, GameObject arg2)
    {
        //DDebug.Log("Equipment changed");
    }
    
   
}
