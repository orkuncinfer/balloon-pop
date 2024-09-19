using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExpDrop : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private EventField _onGameEnded;
    
    private Transform _targetTransform;
    private PlayerActor _playerActor;
    private bool _gameEnded;
    private Vector3 _targetPosition;
    private void OnEnable()
    {
        _playerActor = ActorRegistry.PlayerActor;
        _targetTransform = _playerActor.transform;
        _gameEnded = false;
        _onGameEnded.Register(null,OnGameEnded);
    }

    private void OnDisable()
    {
        _onGameEnded.Unregister(null,OnGameEnded);
    }

    private void OnGameEnded(EventArgs obj)
    {
        _gameEnded = true;
    }


    private void FixedUpdate()
    {
        if (_targetTransform == null) return;
        float speedRef = _speed;
        if (_gameEnded)
        {
            _targetPosition = _targetTransform.position;
            speedRef = 40;
        }
        else
        {
            _targetPosition = transform.position + Vector3.down;
        }
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition,speedRef * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, _targetTransform.position) < 2f)
        {
            if(!_gameEnded)_playerActor.GetData<DS_PlayerRuntime>().RuntimePlayerLevel.AddExperience(1);
            PoolManager.ReleaseObject(gameObject);
        }

        if (transform.position.y < _targetTransform.position.y - 4f)
        {
            PoolManager.ReleaseObject(gameObject);
        }
    }
}
