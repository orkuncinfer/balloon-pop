using System;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBird : MonoBehaviour
{
    [SerializeField] private DSGetter<DS_Spawner> _spawnerData;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _stopDistance = 0.2f;

    private GameObject _target;
    private bool _targetReached;
    private Vector2 _movementDirection;

    private void OnEnable()
    {
        _spawnerData.GetData();

        List<Balloon> balloonList = _spawnerData.Data.BalloonsInBounds;

        _target = GetTarget(balloonList, 3);
        if (_target == null)
        {
            PoolManager.ReleaseObject(this.gameObject);
            return;
        }

        // Set initial movement direction
        _movementDirection = (_target.transform.position - transform.position).normalized;
    }

    private void Update()
    {

        if (!_targetReached)
        {
            MoveTowardsTarget();
            Debug.Log("moving1");
        }
        else
        {
            Debug.Log("moving2");
            ContinueMoving();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 targetPos = _target.transform.position;
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;

        // Move towards target
        transform.position = Vector2.MoveTowards(currentPos, targetPos, _speed * Time.deltaTime);

        // Rotate towards target
        RotateTowards(direction);

        // Check if the bird has reached the target
        if (Vector2.Distance(currentPos, targetPos) <= _stopDistance)
        {
            _targetReached = true;
            _movementDirection = direction; // Save last movement direction
        }
    }

    private void ContinueMoving()
    {
        // Move in the last saved direction
        transform.position += (Vector3)transform.right * _speed * Time.deltaTime;
    }

    private void RotateTowards(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private GameObject GetTarget(List<Balloon> targets, float yThreshold)
    {
        GameObject lowestTarget = null;
        float lowestY = float.MaxValue;

        foreach (Balloon target in targets)
        {
            if (target == null) continue;

            float targetY = target.transform.position.y;

            if (targetY > yThreshold && targetY < lowestY)
            {
                lowestY = targetY;
                lowestTarget = target.gameObject;
            }
        }

        return lowestTarget;
    }
}
