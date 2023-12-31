using Sirenix.OdinInspector;
using UnityEngine;

public class DS_TileCell : Data
{
    [SerializeField][ReadOnly] private Actor _occupiedActor;
    public Actor OccupiedActor  
    {
        get => _occupiedActor;
        set => _occupiedActor = value;
    }
    
    [SerializeField] private bool _isFillable;
    public bool IsFillable => _isFillable;
    
    [SerializeField] private Vector2Int _tileCoordinates;
    public Vector2Int TileCoordinates  
    {
        get => _tileCoordinates;
        set => _tileCoordinates = value;
    }
    
    [SerializeField] private DS_TileBoard _boardData;
    public DS_TileBoard BoardData  
    {
        get => _boardData;
        set => _boardData = value;
    }
}
    
