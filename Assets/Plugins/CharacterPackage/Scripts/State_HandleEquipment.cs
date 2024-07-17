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
        
        EquipCurrent();
        _equipmentData.onEquipmentPrefabChanged += OnEquipmentPrefabChanged;
    }

    private void OnEquipmentPrefabChanged(GameObject arg1, GameObject arg2)
    {
        DDebug.Log("equipment changed");
        PoolManager.ReleaseObject(_equipmentData.EquipmentInstance);
        
        EquipCurrent();
    }
    
    private void EquipCurrent()
    {
        _equipmentData.EquipmentInstance = PoolManager.SpawnObject(_equipmentData.EquipmentPrefab, Vector3.zero, Quaternion.identity);
        
        _equipmentData.EquipmentInstance.transform.SetParent(Owner.GetSocket(_equipmentData.SocketName));
        _equipmentData.EquipmentInstance.transform.localPosition = Vector3.zero;
        _equipmentData.EquipmentInstance.transform.localEulerAngles = Vector3.zero;

        if (_equipmentData.EquipmentInstance.TryGetComponent(out Weapon weapon))
        {
            weapon.Owner = Owner;
        }
    }
}
