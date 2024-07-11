using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ProjectileDetectionState : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private float detectionDistance = 50f;
    [SerializeField] private GameObject visual;
    [SerializeField] private bool _returnIfOutsideOfScreen = true;
    private bool canHit = true;
    private Vector3 positionFirstFrame;
    private float distanceTraveledStep;
    private Transform lockedCollideTransform;
    private Vector2 hitPoint;
    private Collider2D hitCollider;

    private Camera _camera;

    void OnEnable()
    {
        lockedCollideTransform = null;
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

        if (lockedCollideTransform != null)
        {
            Debug.DrawLine(hitPoint, hitPoint + Vector2.up, Color.green, 10);
            if (Vector2.Distance(transform.position, hitPoint) <= 1)
            {
                CollidedWith(hitCollider,hitPoint);
            }
        }
    }

    void Update()
    {
        transform.position += transform.up * Time.deltaTime * moveSpeed;
        
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        
        bool isOutsideOfScreen = screenPosition.x < 0 ||
                                 screenPosition.x > Screen.width ||
                                 screenPosition.y < 0 ||
                                 screenPosition.y > Screen.height;
        
        if(isOutsideOfScreen && _returnIfOutsideOfScreen)
        {
            transform.GetComponent<GOPoolMember>().ReturnToPool();
        }
        
        if (distanceTraveledStep == 0)
        {
            distanceTraveledStep = Vector2.Distance(positionFirstFrame, transform.position);
            //Debug.Log("DistanceTraveledStepCalculated: " + distanceTraveledStep);
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

        if (lockedCollideTransform != null)
        {
            Debug.DrawLine(hitPoint, hitPoint + Vector2.up, Color.green, 10);
            if (Vector2.Distance(transform.position, hitPoint) <= distanceTraveledStep)
            {
                CollidedWith(hitCollider,hitPoint);
            }
        }
    }

    private void CollidedWith(Collider2D collider, Vector2 hitPoint)
    {
        if (!canHit) return;
        canHit = false;
        
        if (collider != null)
        {
            if(collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(1);
            }
        }
        if(gameObject.activeSelf)
            StartCoroutine(WaitAndReturnToPool());
    }
    
    IEnumerator WaitAndReturnToPool()
    {
        transform.position = hitPoint;
        yield return null;
        transform.GetComponent<GOPoolMember>().ReturnToPool();
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
