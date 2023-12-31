using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Levels/NewLevel", fileName = "NewLevel")]
public class LevelDataSO : ScriptableObject
{
    public int BoardWidth;
    public int BoardHeight;
    public  List<GenericKey> LevelDropTypeKeys = new List<GenericKey>();
    public SerializableDictionary<Vector2Int, GenericKey> BoardDropsDictionary;
    public SerializableDictionary<Vector2Int, GenericKey> BoardCellsDictionary;

     [Button]
    public void ClearGridsData()
    {
        BoardDropsDictionary.Clear();
        BoardCellsDictionary.Clear();
    }
}