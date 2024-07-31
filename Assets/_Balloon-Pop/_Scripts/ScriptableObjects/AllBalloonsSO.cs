using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "AllTileTypesAsset", fileName = "AllTileTypesAsset")]
public class AllBalloonsSO : ScriptableObject
{
    public List<ItemBaseDefinition> TileDropTypes;
    public Dictionary<ItemBaseDefinition, TileDropType> TileDropTypesDict = new Dictionary<ItemBaseDefinition, TileDropType>();
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