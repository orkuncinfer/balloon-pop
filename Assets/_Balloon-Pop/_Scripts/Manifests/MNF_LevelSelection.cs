using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MNF_LevelSelection : DataManifest
{
    [SerializeField] private DS_LevelSelection _levelSelection;
    
    protected override Data[] InstallData()
    {
        return new Data[] { _levelSelection };
    }
}

[System.Serializable]
public class DS_LevelSelection : Data
{
    public int LevelCountPerPat;
    
    public List<LevelNode> LevelNodes = new List<LevelNode>();
}