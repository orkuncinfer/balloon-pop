using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class IsInsideScreen : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _registered;
    private Camera _camera;
    
    public event Action<IsInsideScreen> onInsideScreen;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void StartChecking()
    {
        if (!_registered)
        {
            StaticUpdater.onUpdate += IsSpriteCompletelyVisible;
            _registered = true;
        }
    }
    
    public void StopChecking()
    {
        if (_registered)
        {
            StaticUpdater.onUpdate -= IsSpriteCompletelyVisible;
            _registered = false;
        }
    }

    [Button]
    void IsSpriteCompletelyVisible()
    {
        // Get the bounds of the sprite
        Bounds bounds = _spriteRenderer.bounds;

        // Check if the corners of the bounds are within the camera view
        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        corners[1] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        corners[2] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        corners[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);

        foreach (Vector3 corner in corners)
        {
            Vector3 viewportPoint = _camera.WorldToViewportPoint(corner);
            if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
            {
                //Debug.Log("not visible");
                return;
            }
        }
        //Debug.Log("visible");
        onInsideScreen?.Invoke(this);
    }

    private void OnDisable()
    {
        StopChecking();
        onInsideScreen?.Invoke(this);
    }

    private void OnDestroy()
    {
        StopChecking();
        onInsideScreen?.Invoke(this);
    }
}
