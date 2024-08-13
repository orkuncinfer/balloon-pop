using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using WolarGames.Variables;

public class State_SpawnEnemies : MonoState
{
   [SerializeField] private EventField<GameObject> _onBalloonDied;
   [SerializeField] private EventField _requestLevelComplete;
   [SerializeField] private BP_AllLevelListSO _allLevelData;
   [SerializeField] private WaveDataSO _waveData;
   [SerializeField] private Transform _spawnParent;
   [SerializeField] private FloatVariable _globalSpeedFactor;

   //layout settings & spawn settings
   [SerializeField] private float _verticalSpacing;
   [SerializeField] private float _horizontalSpacing;
   [SerializeField] private float _delayBetweenWaves;
   private float _timeSinceLastWave;
   
   private readonly List<GameObject> _spawnedBalloons = new List<GameObject>();

   
   [ShowInInspector]public int SpawnedBalloonsCount => _spawnedBalloons.Count;
   private bool _listeningOneBalloon;
   private bool _canSpawnNextWave;
   private int _currentWaveIndex;
   
   private DS_GameModePersistent _gameModePersistent;
   private DS_Spawner _spawnerData;
   protected override void OnEnter()
   {
      base.OnEnter();
      _gameModePersistent = Owner.GetData<DS_GameModePersistent>();
      _spawnerData = Owner.GetData<DS_Spawner>();
      _canSpawnNextWave = true;
      _currentWaveIndex = 0;
      
      _onBalloonDied.Register(null,OnBalloonDied);
   }

   private void OnBalloonDied(EventArgs arg1, GameObject arg2)
   {
      if (_spawnedBalloons.Contains(arg2))
      {
         _spawnedBalloons.Remove(arg2);
      }
      BP_LevelDataSO levelData = GetLevelData();
      bool isLastWave = _currentWaveIndex >= levelData.Waves.Length;
      bool isLastBalloon = _spawnedBalloons.Count == 0;
      if(isLastBalloon && isLastWave)
      {
         _requestLevelComplete.Raise();
      }
   }

   protected override void OnExit()
   {
      base.OnExit();
      foreach (var balloon in _spawnedBalloons)
      {
         PoolManager.ReleaseObject(balloon);
      }
            
      _onBalloonDied.Unregister(null,OnBalloonDied);
   }
   protected override void OnUpdate()
   {
      base.OnUpdate();
      if ((Time.time >= _timeSinceLastWave || _spawnerData.BalloonsInBounds.Count == 0) && _canSpawnNextWave )
      {
         SpawnWave();
      }
      
      if(_spawnerData.BalloonsInBounds.Count == 0)
      {
         _globalSpeedFactor.CurrentValue = 0.15f;
      }
   }

   private void SpawnWave()
   {
      BP_LevelDataSO levelData = GetLevelData();
      if (_currentWaveIndex >= levelData.Waves.Length)
      {
         return;
      }
      _waveData = levelData.Waves[_currentWaveIndex];
      Vector2Int mostTopMember = FindTheMostTopMember(_waveData);
      for (int i = 0; i < _waveData.BoardWidth; i++)
      {
         for (int j = 0; j < _waveData.BoardHeight; j++)
         {
            if(_waveData.BoardDropsDictionary.Get(new Vector2Int(i, j)) == null) continue;
            GameObject spawnPrefab = null;
            if( _waveData.BoardDropsDictionary.Get(new Vector2Int(i, j)) is ItemDefinition itemDefinition)
            {
               spawnPrefab = itemDefinition.WorldPrefab;
            }
            float xOffset = (_horizontalSpacing * (_waveData.BoardWidth / 2f)) - _horizontalSpacing / 2f;
            GameObject go = PoolManager.SpawnObject(spawnPrefab, _spawnParent.position, Quaternion.identity);
            go.GetComponent<Balloon>().SetSpriteOrder(-1*j);
            if(!_spawnedBalloons.Contains(go)) _spawnedBalloons.Add(go);
            go.transform.SetParent(_spawnParent);
            go.transform.localPosition = new Vector3((i * _horizontalSpacing) - xOffset, j * _verticalSpacing, 0);
            
            if(i == mostTopMember.x && j == mostTopMember.y)
            {
               go.GetComponent<IsInsideScreen>().onInsideScreen += OnLastMemberEnteredScreen;
               go.GetComponent<IsInsideScreen>().StartChecking();
            }
         }
      }
      _currentWaveIndex++;
      _canSpawnNextWave = false;
   }

   private void OnLastMemberEnteredScreen(IsInsideScreen obj)
   {
      _canSpawnNextWave = true;
      _timeSinceLastWave = Time.time + _delayBetweenWaves;
      obj.onInsideScreen -= OnLastMemberEnteredScreen;
      obj.StopChecking();
   }
   
   private Vector2Int FindTheMostTopMember(WaveDataSO waveData)
   {
      Vector2Int topMember = new Vector2Int();

      for (int i = waveData.BoardHeight-1; i > 0 ; i--)
      {
         for (int j = 0; j < waveData.BoardWidth; j++)
         {
            if (waveData.BoardDropsDictionary.Get(new Vector2Int(j, i)) != null)
            {
               topMember = new Vector2Int(j, i);
               return topMember;
            }
         }
      }
      return topMember;
   }

   private BP_LevelDataSO GetLevelData()
   {
      BP_LevelDataSO levelData = null;
      if (_gameModePersistent.CurrentLevelIndex >= _allLevelData.Items.Count)
      {
         int mod = _gameModePersistent.CurrentLevelIndex % _allLevelData.Items.Count;
         levelData = _allLevelData.Items[mod];
      }
      else
      {
         levelData = _allLevelData.Items[_gameModePersistent.CurrentLevelIndex];
      }
      Debug.Log("GetLevelData Name is " + levelData.name);
      return levelData;
   }
}
