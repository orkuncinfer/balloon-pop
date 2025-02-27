using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class MNF_PlayerRuntime : DataManifest
{
    [SerializeField] private DS_PlayerRuntime _playerRuntime;
    
    protected override Data[] InstallData()
    {
        return new Data[] { _playerRuntime};
    }
    [Button]
    public void Test()
    {
        Debug.Log(Actor.GetData<DS_PlayerRuntime>().CurrentHealth);
    }
}

[System.Serializable]
public class DS_PlayerRuntime : Data
{
    public BP_PlayerLevelableSO RuntimePlayerLevel;
    
    [SerializeField]
    private int _currentHealth; 
    public event Action<int,int> onCurrentHealthChanged;
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            int oldValue = _currentHealth;
            bool isChanged = _currentHealth != value;
            _currentHealth = value;
            if (isChanged)
            {
                onCurrentHealthChanged?.Invoke(oldValue,value);
            }
        }
    }
    
    [SerializeField]
    private float _attackSpeedRuntime; 
    public float AttackSpeedRuntime 
    {
        get => _attackSpeedRuntime;
        set => _attackSpeedRuntime = value;
    }

    protected override void OnActorStarted()
    {
        base.OnActorStarted();
        GlobalData.SubscribeToDataInstalled(OnDataInstalledHandler, "", typeof(DS_PlayerPersistent));
    }

    private void OnDataInstalledHandler(Data obj)
    {
        CurrentHealth = GlobalData.GetData<DS_PlayerPersistent>().MaxHealth;
    }

}
