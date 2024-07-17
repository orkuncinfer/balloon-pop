using System;
using System.Collections;
using System.Collections.Generic;
using AudioSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Balloon : MonoBehaviour, IDamageable
{
    public ItemDefinition ItemDefinition;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GameObject _deadFx;
    [ReadOnly][SerializeField] private int _currentHealth;
    [ReadOnly][SerializeField] private int _moveSpeed;
    private void OnEnable()
    {
        _currentHealth = ItemDefinition.GetData<DataVar_Int>(TagEnum.Health.ToString()).Value;
        _moveSpeed = ItemDefinition.GetData<DataVar_Int>(TagEnum.MoveSpeed.ToString()).Value;
    }

    private void FixedUpdate()
    {
        //transform.position += Vector3.down * (_moveSpeed * Time.deltaTime);
        _rigidbody.MovePosition(transform.position + Vector3.down * (_moveSpeed * Time.deltaTime));
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            //PoolMember.ReturnToPool();
            PoolManager.SpawnObject(_deadFx, transform.position, Quaternion.identity);
            PoolManager.ReleaseObject(gameObject);
            SoundManager.Instance.CreateSoundBuilder().WithRandomPitch().Play(SoundManager.Instance.Container.BalloonPop);
        }
    }
}
