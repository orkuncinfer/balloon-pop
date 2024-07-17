using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_SpawnLevelPath : MonoState
{
    [SerializeField] private GameObject _pathPrefab;
    [SerializeField] private int _totalLevelCount;
    [SerializeField] private int _levelPerPath;
    [SerializeField] private RectTransform _pathParent;
    [SerializeField] private float _heightOffset;
    [SerializeField] private float _startY;
    private bool _spawned;

    protected override void OnEnter()
    {
        base.OnEnter();
        if(_spawned) return;
        SpawnLevelPaths();
    }

    private void SpawnLevelPaths()
    {
        int pathCount = Mathf.CeilToInt((float)_totalLevelCount / _levelPerPath);
        for (int i = 0; i < pathCount; i++)
        {
            GameObject path = Instantiate(_pathPrefab, _pathParent);
           /* RectTransform rectTransform = path.GetComponent<RectTransform>();
            if(i == 0){rectTransform.localPosition = new Vector2(0, _startY);}
            else
            {
                rectTransform.localPosition = new Vector2(0, _startY + (i * rectTransform.rect.height) + (i * _heightOffset));
            }*/
            
            LevelPath levelPath = path.GetComponent<LevelPath>();
            levelPath.StartLevelNumber = (i * _levelPerPath) + 1;
            levelPath.UpdateLevelNodes();
        }
        _spawned = true;
    }
}