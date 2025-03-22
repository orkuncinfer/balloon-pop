using System;
using UnityEngine;

public class TriggerDamageBalloon : MonoBehaviour
{
    public int Damage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered");
        if(other.TryGetComponent(out Balloon balloon))
        {
            int damage = balloon.ItemDefinition.GetData<DataVar_Int>(TagEnum.Damage.ToString()).Value;
            balloon.TakeDamage(Damage);
        }
    }
}
