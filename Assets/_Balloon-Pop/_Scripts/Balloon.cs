using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Balloon : MonoBehaviour, IDamageable
{
    public ItemDefinition ItemDefinition;
    
    [ReadOnly][SerializeField] private int _currentHealth;
    [ReadOnly][SerializeField] private int _moveSpeed;
    public GOPoolMember PoolMember;
    private void OnEnable()
    {
        if(PoolMember == null)
            PoolMember = GetComponent<GOPoolMember>();
        _currentHealth = ItemDefinition.GetData<DataVar_Int>(TagEnum.Health.ToString()).Value;
        _moveSpeed = ItemDefinition.GetData<DataVar_Int>(TagEnum.MoveSpeed.ToString()).Value;
    }


    private void Update()
    {
        transform.position += Vector3.down * (_moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            //PoolMember.ReturnToPool();
            Debug.Log("transform : " + transform.position + " has been destroyed!");
        }
    }
}
