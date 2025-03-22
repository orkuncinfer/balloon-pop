using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SideDartsBuffHandler : MonoCore
{
    [SerializeField] private GameObject _throwerPrefab;
    [SerializeField] private Transform _pivot1;
    [SerializeField] private Transform _pivot2;
    [SerializeField] private Transform _pivot3;

    private bool _pivot1Filled;
    private bool _pivot2Filled;
    private bool _pivot3Filled;
    
    [SerializeField] private EventField<string> _onSideDartsBuffSelected;
    [SerializeField] private ItemDefinition _sideDartsBuffItem;

    private Camera _camera;
    protected override void OnGameReady()
    {
        base.OnGameReady();
        _camera = Camera.main;
        _onSideDartsBuffSelected.Register(null,OnSelected);
    }

    protected override void OnGameModeStopped()
    {
        base.OnGameModeStopped();
        _onSideDartsBuffSelected.Unregister(null,OnSelected);
        _pivot1Filled = false;
        _pivot2Filled = false;
        _pivot3Filled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSelected(new EventArgs(),_sideDartsBuffItem.ItemId);
        }
    }

    [Button]
    private void OnSelected(EventArgs arg1, string arg2)
    {
        if(arg2 != _sideDartsBuffItem.ItemId)return;
        Vector3 spawnPosition = Vector3.zero;
        if (!_pivot1Filled)
        {
            spawnPosition = _pivot1.position;
            _pivot1Filled = true;
        }
        else if (!_pivot2Filled)
        {
            spawnPosition = _pivot2.position;
            _pivot2Filled = true;
        }
        else if (!_pivot3Filled)
        {
            spawnPosition = _pivot3.position;
            _pivot3Filled = true;
        }
        else
        {
            return;
        }
        GameObject leftThrower = PoolManager.SpawnObject(_throwerPrefab,spawnPosition, Quaternion.identity);
        GameObject rightThrower = PoolManager.SpawnObject(_throwerPrefab,spawnPosition, Quaternion.identity);
        
        float screenLeftBoundInWorldX = _camera.ScreenToWorldPoint(new Vector3(0,0,0)).x;
        float screenRightBoundInWorldX = _camera.ScreenToWorldPoint(new Vector3(Screen.width,0,0)).x;
        Debug.Log("Screen Left Bound: " + screenRightBoundInWorldX +":" +Screen.width);
        leftThrower.transform.position = new Vector3(screenLeftBoundInWorldX,spawnPosition.y,spawnPosition.z);
        rightThrower.transform.position = new Vector3(screenRightBoundInWorldX,spawnPosition.y,spawnPosition.z);
        rightThrower.transform.rotation = Quaternion.Euler(0,0,180);
    }

   
}