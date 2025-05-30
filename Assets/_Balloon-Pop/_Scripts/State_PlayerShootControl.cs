using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using MyFastProjectile;
using UnityEngine;

public class State_PlayerShootControl : MonoState
{
    [SerializeField] private PlayerInputDefinition _playerInputDefinition;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint;
    
    private DS_PlayerPersistent _playerData;
    private DS_PlayerRuntime _playerRuntime;

    private float _lastShootTime;
    private Camera _camera;

    protected override void OnEnter()
    {
        base.OnEnter();
        _playerData = DataGetter.GetData<DS_PlayerPersistent>();
        _playerRuntime = ActorRegistry.PlayerActor.GetData<DS_PlayerRuntime>();
        _camera = Camera.main;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(_playerData == null) return;
        if(Time.time > _lastShootTime )
        {
            
            Shoot();
            _lastShootTime = Time.time + 1 /( _playerRuntime.AttackSpeedRuntime + _playerData.AttackSpeedPersistent);
        }
        Vector3 movementDelta = new Vector3(_playerInputDefinition.MoveInput.x, 0, 0) * (Time.deltaTime * 2);
        Owner.transform.position += movementDelta;
        if (_camera != null)
        {
            Vector2 screenPosition = _camera.WorldToScreenPoint(Owner.transform.position);
            if (screenPosition.x < 70)
            {
                Vector2 worldPosition = _camera.ScreenToWorldPoint(new Vector2(70, screenPosition.y));
                Owner.transform.position = worldPosition;
            }else if (screenPosition.x > Screen.width-70)
            {
                Vector2 worldPosition = _camera.ScreenToWorldPoint(new Vector2(Screen.width-70, screenPosition.y));
                Owner.transform.position = worldPosition;
            }
            
            
        }
        
    }

    private void Shoot()
    {
        GameObject spawned = PoolManager.SpawnObject(_projectilePrefab, _shootPoint.position, Quaternion.identity);
        spawned.GetComponent<FastProjectile>().Initialize(Vector3.up, false);
    }
}
