using System;
using System.Collections;
using System.Collections.Generic;
using AudioSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WolarGames.Variables;

public class Balloon : MonoBehaviour, IDamageable
{

    public ItemDefinition ItemDefinition;
    public float DesiredYPosition = 9.6f;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _balloonSprite;
    [SerializeField] private GameObject _deadFx;
    [ReadOnly][SerializeField] private int _currentHealth;
    [ReadOnly][SerializeField] private int _moveSpeed;
    [SerializeField]private FloatVariable _globalSpeedFactor;
    [SerializeField] private GameObject _expDropPrefab;
    
    [SerializeField] private EventField<GameObject> _onBalloonDied;
    [SerializeField] private UnityEvent _onBalloonDiedAction;
    [SerializeField] private UnityEvent _onBalloonTakeDamageAction;
    
    private DS_Spawner _spawnerData;
    private void OnEnable()
    {
        _currentHealth = ItemDefinition.GetData<DataVar_Int>(TagEnum.Health.ToString()).Value;
        _moveSpeed = ItemDefinition.GetData<DataVar_Int>(TagEnum.MoveSpeed.ToString()).Value;
        _spawnerData = GlobalData.GetData<DS_Spawner>();
    }
    

    private void FixedUpdate()
    {
        //transform.position += Vector3.down * (_moveSpeed * Time.deltaTime);
        if (transform.position.y < 15)
        {
            if(!_spawnerData.BalloonsInBounds.Contains(this))
                _spawnerData.BalloonsInBounds.Add(this);
        }

        if (transform.position.y < DesiredYPosition)
        {
            _globalSpeedFactor.CurrentValue = 0;
        }
        
        _rigidbody.MovePosition(transform.position + Vector3.down * ((_moveSpeed * Time.deltaTime) + _globalSpeedFactor.CurrentValue));
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            //PoolMember.ReturnToPool();
            _onBalloonDied.Raise(gameObject);
            _onBalloonDiedAction.Invoke();
            PoolManager.SpawnObject(_deadFx, transform.position, Quaternion.identity);
            PoolManager.SpawnObject(_expDropPrefab, transform.position, Quaternion.identity);
            PoolManager.ReleaseObject(gameObject);
            SoundManager.Instance.CreateSoundBuilder().WithRandomPitch().Play(SoundManager.Instance.Container.BalloonPop);
        }
        _onBalloonTakeDamageAction.Invoke();
    }
    
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
    }

    public void SetSpriteOrder(int order)
    {
        _balloonSprite.sortingOrder = order;
    }

    private void OnDisable()
    {
        if(_spawnerData.BalloonsInBounds.Contains(this))
            _spawnerData.BalloonsInBounds.Remove(this);
    }

    private void OnDestroy()
    {
        if(_spawnerData.BalloonsInBounds.Contains(this))
            _spawnerData.BalloonsInBounds.Remove(this);
    }
}
