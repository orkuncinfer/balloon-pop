using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public bool DrawDebug;
    public LayerMask LayerMask;
    public BoxCollider Collider;

    public event Action<Collider> onHit;
    
    public void Cast()
    {
        Vector3 worldCenter = Collider.transform.TransformPoint(Collider.center);
        Vector3 worldHalfExtents = Collider.size * 0.5f; // only necessary when collider is scaled by non-uniform transform
        float maxDistance = 10f; // Max distance for the cast
        RaycastHit hitInfo;

        // Perform the BoxCast
       
        
        Collider[] hitColliders = new Collider[10];  // Adjust the size as necessary
        int numColliders = Physics.OverlapBoxNonAlloc(worldCenter, worldHalfExtents, hitColliders, transform.rotation, LayerMask);

        for (int i = 0; i < numColliders; i++)
        {
            if(hitColliders[i].transform == Owner.transform) continue;
            //DDebug.Log("Hit : " + hitColliders[i].name + i);
            if(hitColliders[i].transform.TryGetComponent(out Actor actor))
            {
                onHit?.Invoke(hitColliders[i]);
            }
        }
        // ExtDebug.DrawBox(center,halfExtents,transform.rotation,Color.red);
    
    }
}
