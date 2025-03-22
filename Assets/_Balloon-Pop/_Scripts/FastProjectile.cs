using System.Collections;
using System.Collections.Generic;
using Oddworm.Framework;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

public class FastProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private float detectionDistance = 50f;
    [SerializeField] private GameObject visual;
    [SerializeField] private bool _returnIfOutsideOfScreen = true;
    [SerializeField] private ItemDefinition _ricochet2Item;
    [SerializeField] private ItemDefinition _ricochet4Item;
    [SerializeField] private ItemDefinition _pierce1Item;
    [SerializeField] private ItemDefinition _pierce2Item;
    private bool canHit = true;
    private Vector3 positionFirstFrame;
    private float distanceTraveledStep;
    private Transform lockedCollideTransform;
    private Vector2 hitPoint;
    private Collider2D hitCollider;

    private Camera _camera;

    private HashSet<string> _hitRegistry = new HashSet<string>();

    private bool _hasPierce => false;

    private Vector3 _moveDirection;
    private bool _isSubProjectile;

    private int _health;

    void OnEnable()
    {
        _hitRegistry.Clear();
        _moveDirection = Vector3.zero;
        _isSubProjectile = false;
        lockedCollideTransform = null;
        hitCollider = null;
        _camera = Camera.main;
        positionFirstFrame = transform.position;
        visual.SetActive(true);
        canHit = true;
        distanceTraveledStep = 0;

        //initial hit check
        bool isHit = Physics2D.Raycast(transform.position, transform.up, detectionDistance, hitLayerMask);
        if (isHit)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, detectionDistance, hitLayerMask);
            if (hitInfo && Vector2.Distance(transform.position, hitInfo.point) <= 1)
            {
                lockedCollideTransform = hitInfo.transform;
                hitPoint = hitInfo.point;
                hitCollider = hitInfo.collider;
            }
        }

        if (lockedCollideTransform != null && hitCollider != null)
        {
            Debug.DrawLine(hitPoint, hitPoint + Vector2.up, Color.green, 1);
            if (Vector2.Distance(transform.position, hitPoint) <= 1)
            {
                CollidedWith(hitCollider, hitPoint);
            }
        }
    }

    public void Initialize(Vector3 dir, bool isSubProjectile)
    {
        transform.up = dir;
        _moveDirection = dir;
        _isSubProjectile = isSubProjectile;
        if (BuffValidator.HasBuff(_pierce2Item))
        {
            _health = 3;
        }else if (BuffValidator.HasBuff(_pierce1Item))
        {
            _health = 2;
        }
        else
        {
            _health = 1;
        }

        if (isSubProjectile) _health = 1;
        Debug.Log("Projectile health :" + _health);
    }

    void Update()
    {
        if (_moveDirection == Vector3.zero) _moveDirection = Vector3.up;
        transform.position += _moveDirection * Time.deltaTime * moveSpeed;
        transform.rotation =
            Quaternion.Euler(0, 0, Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg - 90);
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        bool isOutsideOfScreen = screenPosition.x < 0 ||
                                 screenPosition.x > Screen.width ||
                                 screenPosition.y < 0 ||
                                 screenPosition.y > Screen.height;

        if (isOutsideOfScreen && _returnIfOutsideOfScreen)
        {
            PoolManager.ReleaseObject(gameObject);
        }

        if (distanceTraveledStep == 0)
        {
            distanceTraveledStep = Vector2.Distance(positionFirstFrame, transform.position);
        }


        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, detectionDistance, hitLayerMask);
        Debug.DrawRay(transform.position, transform.up * detectionDistance, Color.blue);

        if (hitInfo && Vector2.Distance(transform.position, hitInfo.point) <= distanceTraveledStep * 2)
        {
            lockedCollideTransform = hitInfo.transform;
            hitPoint = hitInfo.point;
            hitCollider = hitInfo.collider;
        }

        //Debug.Log("DistanceChecked: " + (hitInfo.collider != null) + " " + Vector2.Distance(transform.position, hitInfo.point) + " > " + distanceTraveledStep * 2);

        if (lockedCollideTransform != null && hitCollider != null)
        {
            Debug.DrawLine(hitPoint, hitPoint + Vector2.up, Color.green, 1);
            if (Vector2.Distance(transform.position, hitPoint) <= distanceTraveledStep)
            {
                CollidedWith(hitCollider, hitPoint);
            }
        }
    }

    private void CollidedWith(Collider2D collider, Vector2 hitPoint)
    {
        //Debug.Log($"Collided with {collider.gameObject.name} at {hitPoint} by {gameObject.name}");
        if (_hitRegistry.Contains(collider.gameObject.GetInstanceID().ToString())) return;
        if (!_hasPierce)
        {
            if (!canHit) return;
            
            _health--;
            if (_health <= 0) canHit = false;
        }

        _hitRegistry.Add(collider.gameObject.GetInstanceID().ToString());
        if (collider != null)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(1);

                if (!_isSubProjectile &&
                    (BuffValidator.HasBuff(_ricochet2Item) || BuffValidator.HasBuff(_ricochet4Item)))
                {
                    Transform[] transforms = null;
                    if (BuffValidator.HasBuff(_ricochet4Item))
                    {
                        transforms = FindNearestTransforms(transform.position, 5, hitLayerMask, 3);
                    }
                    else if(BuffValidator.HasBuff(_ricochet2Item))
                    {
                        transforms = FindNearestTransforms(transform.position, 2, hitLayerMask, 1);
                    }
                    foreach (var target in transforms)
                    {
                        GameObject spawned =
                            PoolManager.SpawnObject(gameObject, transform.position, Quaternion.identity);
                        Vector3 directionToTarget = target.position - transform.position;
                        directionToTarget.Normalize();
                        spawned.GetComponent<FastProjectile>().Initialize(directionToTarget, true);
                        DbgDraw.Line(transform.position, target.position, Color.green, 1);
                    }
                }
            }
        }

        lockedCollideTransform = null;
        hitCollider = null;

      
        if (gameObject.activeSelf && !_hasPierce)
            StartCoroutine(WaitAndReturnToPool());
    }

    IEnumerator WaitAndReturnToPool()
    {
        transform.position = hitPoint;
        yield return null;
        if (_health <= 0)
        {
            PoolManager.ReleaseObject(gameObject);
            StopAllCoroutines();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }

    public Transform[] FindNearestTransforms(Vector2 center, float radius, LayerMask layerMask, int numberOfNearest)
    {
        // Perform the circle cast
        Collider2D[] results = new Collider2D[50];
        int count = Physics2D.OverlapCircleNonAlloc(center, radius, results, layerMask);
        DbgDraw.WireDisc(center, Quaternion.Euler(90, 0, 0), radius, Color.red, 1);

        SortedList<float, Transform> nearestTransforms =
            new SortedList<float, Transform>(new DuplicateKeyComparer<float>());

        for (int i = 0; i < count; i++)
        {
            var collider = results[i];
            float distance = Vector2.Distance(center, collider.transform.position);

            // Add the transform to the sorted list
            nearestTransforms.Add(distance, collider.transform);

            // Keep the list to the specified number of nearest objects
            if (nearestTransforms.Count > numberOfNearest)
            {
                nearestTransforms.RemoveAt(nearestTransforms.Count - 1);
            }
        }

        // Extract and return the transforms from the sorted list
        Transform[] nearestArray = new Transform[nearestTransforms.Count];
        nearestTransforms.Values.CopyTo(nearestArray, 0);

        return nearestArray;
    }
}

public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : System.IComparable
{
    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        // Handle equality as being greater (to allow duplicates)
        if (result == 0)
            return 1;
        else
            return result;
    }
}