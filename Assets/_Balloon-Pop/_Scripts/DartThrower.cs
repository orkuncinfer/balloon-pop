using System;
using UnityEngine;

public class DartThrower : MonoCore
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint;

    [SerializeField] private float _attackSpeed;
    
    private DS_PlayerPersistent _playerData;

    private float _lastShootTime;

    private void Update()
    {
        if(Time.time > _lastShootTime )
        {
            
            Shoot();
            _lastShootTime = Time.time + 1 /( _attackSpeed );
        }
    }
    private void Shoot()
    {
        GameObject spawned = PoolManager.SpawnObject(_projectilePrefab, _shootPoint.position, Quaternion.identity);
        //spawned.transform.up = transform.right;
        spawned.GetComponent<FastProjectile>().Initialize(transform.right,true);
    }

    protected override void OnGameModeStopped()
    {
        base.OnGameModeStopped();
        PoolManager.ReleaseObject(gameObject);
    }
}