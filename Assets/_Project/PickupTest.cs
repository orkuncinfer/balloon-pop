using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTest : MonoBehaviour
{
    public Actor UserActor;
    
    public GameObject EquipmentInstance;
    
    public void PickedUp()
    {
        UserActor.GetData<DS_EquipmentUser>().EquipmentPrefab = EquipmentInstance;
    }
}
