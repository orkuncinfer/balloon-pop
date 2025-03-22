using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MNF_Spawner : DataManifest
{
    [SerializeField] private DS_Spawner _spawnerData;
    
    protected override Data[] InstallData()
    {
        return new Data[] {_spawnerData};
    }
}
[Serializable]
public class DS_Spawner : Data
{
    public List<Balloon> BalloonsInBounds => _balloonsInBounds;
    [ShowInInspector]private List<Balloon> _balloonsInBounds = new List<Balloon>();
}
