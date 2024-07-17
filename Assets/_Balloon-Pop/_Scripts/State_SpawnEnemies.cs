using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class State_SpawnEnemies : MonoState
{
   [SerializeField] private WaveDataSO waveData;
   [SerializeField] private GameObject _balloonPrefab;
   [SerializeField] private ItemDefinition _redBalloon;

   [SerializeField] private float _verticalSpacing;
   [SerializeField] private float _horizontalSpacing;

   [SerializeField] private float _delayBetweenWaves;
   private float _timeSinceLastWave;
   
   private List<GameObject> _spawnedBalloons = new List<GameObject>();
   protected override void OnEnter()
   {
      base.OnEnter();
   }

   protected override void OnExit()
   {
      base.OnExit();
      foreach (var balloon in _spawnedBalloons)
      {
         PoolManager.ReleaseObject(balloon);
      }
   }

   [Button]
   public void GetData()
   {
      //Debug.Log("Data: " + _redBalloon.GetData<DataVar_Float>(TagEnum.Health.ToString()).Value);
   }



   protected override void OnUpdate()
   {
      base.OnUpdate();
      if (Time.time >= _timeSinceLastWave)
      {
         SpawnWave();
         _timeSinceLastWave = Time.time + _delayBetweenWaves;
      }
   }

   private void SpawnWave()
   {
      for (int i = 0; i < waveData.BoardWidth; i++)
      {
         for (int j = 0; j < waveData.BoardHeight; j++)
         {
            if(waveData.BoardDropsDictionary.Get(new Vector2Int(i, j)) == null) continue;
            float xOffset = (_horizontalSpacing * (waveData.BoardWidth / 2f)) - _horizontalSpacing / 2f;
            GameObject go = PoolManager.SpawnObject(_balloonPrefab, new Vector3((i * _horizontalSpacing) - xOffset, j * _verticalSpacing, 0), Quaternion.identity);
            if(!_spawnedBalloons.Contains(go)) _spawnedBalloons.Add(go);
            go.transform.SetParent(transform);
         }
      }
   }
}
