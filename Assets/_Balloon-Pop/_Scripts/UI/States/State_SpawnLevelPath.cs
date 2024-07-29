using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class State_SpawnLevelPath : MonoState
{
    [SerializeField] private GameObject _pathPrefab;
    [SerializeField] private int _totalLevelCount;
    [SerializeField] private int _levelPerPath;
    [SerializeField] private RectTransform _pathParent;
    [SerializeField] private float _heightOffset;
    [SerializeField] private float _startY;
    private List<LevelPath> _levelPathList = new List<LevelPath>();
    private bool _spawned;

    private DS_LevelSelection _levelSelection;
    private DS_GameModePersistent _gameModePersistent;

    protected override void OnEnter()
    {
        base.OnEnter();
        _levelSelection = Owner.GetData<DS_LevelSelection>();
        _gameModePersistent = Owner.GetData<DS_GameModePersistent>();

        if (!_spawned) SpawnLevelPaths();
        
        if(_spawned) UpdateLevelPaths();
    }

    private void SpawnLevelPaths()
    {
        int pathCount = Mathf.CeilToInt((float) _totalLevelCount / _levelPerPath);
        for (int i = 0; i < pathCount; i++)
        {
            GameObject path = Instantiate(_pathPrefab, _pathParent);
            LevelPath levelPath = path.GetComponent<LevelPath>();
            levelPath.StartLevelNumber = (i * _levelPerPath) + 1;
            levelPath.UpdateLevelNodes(_gameModePersistent.MaxReachedLevelIndex);
            _levelPathList.Add(levelPath);
            foreach (var node in levelPath.Nodes)
            {
                _levelSelection.LevelNodes.Add(node);
            }
        }
        _spawned = true;
    }
    
    private void UpdateLevelPaths()
    {
        for (int i = 0; i < _levelPathList.Count; i++)
        {
            _levelPathList[i].UpdateLevelNodes(_gameModePersistent.MaxReachedLevelIndex);
        }
    }
}