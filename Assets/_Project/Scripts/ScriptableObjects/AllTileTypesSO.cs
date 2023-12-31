using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "AllTileTypesAsset", fileName = "AllTileTypesAsset")]
public class AllTileTypesSO : ScriptableObject
{
    public List<TileDropType> TileDropTypes;
    public Dictionary<GenericKey, TileDropType> TileDropTypesDict = new Dictionary<GenericKey, TileDropType>();
    public List<TileCellType> TileCellTypes;
    public Dictionary<GenericKey, TileCellType> TileCellTypesDict = new Dictionary<GenericKey, TileCellType>();
    
    public TileCellType GetTileCellType(GenericKey genericKey)
    {
        if (TileCellTypesDict.Count != TileCellTypes.Count)
        {
            TileCellTypesDict.Clear();
            for (int i = 0; i < TileCellTypes.Count; i++)
            {
                TileCellTypesDict.Add(TileCellTypes[i].MachItemRepKey,TileCellTypes[i]);
            }
        }

        return TileCellTypesDict[genericKey];
    }

    public TileDropType GetTileDropType(GenericKey genericKey)
    {
        if (TileDropTypesDict.Count != TileDropTypes.Count)
        {
            TileDropTypesDict.Clear();
            for (int i = 0; i < TileDropTypes.Count; i++)
            {
                TileDropTypesDict.Add(TileDropTypes[i].MachItemRepKey,TileDropTypes[i]);
            }
        }

        if (TileDropTypesDict.ContainsKey(genericKey))
        {
            return TileDropTypesDict[genericKey];
        }
        else
        {
            return null;
        }
        
    }
}


[System.Serializable]
public class TileDropType
{
    public GenericKey MachItemRepKey;
    public GameObject Prefab;
    public GameObject MatchedEffectPrefab;
    [PreviewField]
    public Sprite Visual;
}

[System.Serializable]
public class TileCellType
{
    public GenericKey MachItemRepKey;
    public GameObject Prefab;
    [PreviewField]
    public Sprite Visual;
}