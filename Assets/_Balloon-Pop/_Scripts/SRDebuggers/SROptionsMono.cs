using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SROptionsMono : MonoBehaviour
{
    [SerializeField] private BPSrOptions _options;
    private void Start()
    {
        SRDebug.Init();
        _options.DataManifests = FindObjectsOfType<DataManifest>();
        SRDebug.Instance.AddOptionContainer(_options);
    }
}

[System.Serializable]
public class BPSrOptions
{
    public DataManifest[] DataManifests;
    
    [Category("SaveLoad")]
    public void SaveData()
    {
        foreach (var manifest in DataManifests)
        {
            manifest.SaveData();
        }
    }
    
    [Category("SaveLoad")]
    public void LoadData()
    {
        foreach (var manifest in DataManifests)
        {
            manifest.LoadData();
        }
    }
}

