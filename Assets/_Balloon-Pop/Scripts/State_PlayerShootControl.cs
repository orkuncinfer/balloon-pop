using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class State_PlayerShootControl : MonoState
{
    [SerializeField] private GameObject _projectilePrefab;
    
    private DS_PlayerPersistent _playerData;

    private float _lastShootTime;

    protected override void OnEnter()
    {
        base.OnEnter();
        _playerData = DataGetter.GetData<DS_PlayerPersistent>();
        Debug.Log(_playerData.AttackSpeed);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(_playerData == null) return;
        if(Time.time > _lastShootTime )
        {
            
            Shoot();
            _lastShootTime = Time.time + 1 / _playerData.AttackSpeed;
        }
    }

    private void Shoot()
    {
        PoolProvider.Retrieve(_projectilePrefab, Owner.transform.position, Quaternion.identity);
    }
}
