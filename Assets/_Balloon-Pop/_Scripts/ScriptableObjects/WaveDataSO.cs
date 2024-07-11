using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Levels/NewLevel", fileName = "NewLevel")]
public class WaveDataSO : ScriptableObject
{
    public int BoardWidth;
    public int BoardHeight;
    public  List<ItemBaseDefinition> LevelDropTypeKeys = new List<ItemBaseDefinition>();
    public SerializableDictionary<Vector2Int, ItemBaseDefinition> BoardDropsDictionary;

     [Button]
    public void ClearGridsData()
    {
        BoardDropsDictionary.Clear();
    }
}

public enum DropTypes
{
    Red,
    Blue,
    Green
}