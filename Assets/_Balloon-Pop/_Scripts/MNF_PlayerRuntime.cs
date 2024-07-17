using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
    [SerializeField]
    private int _currentHealth; 
    public int CurrentHealth 
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }
    
    [SerializeField]
    private List<ItemDefinition> _inGameBuffs; 
    public List<ItemDefinition> InGameBuffs 
    {
        get => _inGameBuffs;
        set => _inGameBuffs = value;
    }

    public override void OnActorStarted()
    {
        base.OnActorStarted();
        GlobalData.SubscribeToDataInstalled(OnDataInstalledHandler, "", typeof(DS_PlayerPersistent));
        GlobalData.OnDataInstalled += data =>
        {
            if (data is DS_PlayerPersistent)
            {
                _currentHealth = GlobalData.GetData<DS_PlayerPersistent>().MaxHealth;
            }
        };
    }

    private void OnDataInstalledHandler(Data obj)
    {
        _currentHealth = GlobalData.GetData<DS_PlayerPersistent>().MaxHealth;
    }

}
