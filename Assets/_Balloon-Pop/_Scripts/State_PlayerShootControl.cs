using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class State_PlayerShootControl : MonoState
{
    [SerializeField] private PlayerInputDefinition _playerInputDefinition;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint;
    
    private DS_PlayerPersistent _playerData;

    private float _lastShootTime;
    private Camera _camera;

    protected override void OnEnter()
    {
        base.OnEnter();
        _playerData = DataGetter.GetData<DS_PlayerPersistent>();
        _camera = Camera.main;
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
        PoolManager.SpawnObject(_projectilePrefab, _shootPoint.position, Quaternion.identity);
    }
}
