using System;
using System.Collections.Generic;
using UnityEngine;

public class DataTest : DataGroup
{
    [SerializeField] private DataInstaller<DS_PlayerPersistent> _data;
    
    protected override IEnumerable<IDataInstaller> GetInstallers()
    {
        yield return _data;
    }
    
}