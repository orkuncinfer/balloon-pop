using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class FlyingBirdsBuffHandler : InGameBuff
{
    [SerializeField] private GameObject _birdPrefab;
    [SerializeField] private float SpawnDelay;
    private float _nextSpawnTime;
    private int _spawnedIndex;

    private void Update()
    {
        if(!_isBuffActive) return;
        if (Time.time > _nextSpawnTime)
        {
            SpawnBird();
            _spawnedIndex++;
            _nextSpawnTime = Time.time + SpawnDelay;
        }
    }

    protected override void OnBuffSelected()
    {
        base.OnBuffSelected();
    }

    private void SpawnBird()
    {
        Vector3 spawnPosition;
        float x;
        float yScale;
        if (_spawnedIndex % 2 == 0)
        {
            x = -10;
            yScale = 1;
        }
        else
        {
            x = 10;
            yScale = -1;
        }
        float y = Random.Range(0, 11);
        spawnPosition = new Vector3(x, y, 0);
        
        GameObject bird = PoolManager.SpawnObject(_birdPrefab, spawnPosition, Quaternion.identity);
        bird.transform.localScale = new Vector3(1,yScale,1);
    }
}