using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "AllTileTypesAsset", fileName = "AllTileTypesAsset")]
public class AllTileTypesSO : ScriptableObject
{
    public List<TileDropType> TileDropTypes;
    public Dictionary<ItemBaseDefinition, TileDropType> TileDropTypesDict = new Dictionary<ItemBaseDefinition, TileDropType>();
    

    public TileDropType GetTileDropType(ItemBaseDefinition genericKey)
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
    public ItemBaseDefinition MachItemRepKey;
    public GameObject Prefab;
    public GameObject MatchedEffectPrefab;
    [PreviewField]
    public Sprite Visual;
}

[System.Serializable]
public class TileCellType
{
    public ItemBaseDefinition MachItemRepKey;
    public GameObject Prefab;
    [PreviewField]
    public Sprite Visual;
}