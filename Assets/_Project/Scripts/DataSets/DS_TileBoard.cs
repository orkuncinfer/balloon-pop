using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DS_TileBoard : Data
{
     private int _width;
    public int Width => GetCurrentLevelData().BoardWidth;
    
    private int _height;
    public int Height => GetCurrentLevelData().BoardHeight;
    
    [SerializeField] private float _spacing;
    public float Spacing => _spacing;
    
    [Range(0,1)][SerializeField]private float _fillScreenPercentage = 0.9f;
    public float FillScreenPercentage => _fillScreenPercentage;
    
    [SerializeField] private Transform _tileHolder;
    public Transform TileHolder => _tileHolder;
    
    [SerializeField] private GameObject _dropPrefab;
    public GameObject DropPrefab => _dropPrefab;
    
    [SerializeField] private AllTileTypesSO _allTileRep;
    public AllTileTypesSO AllTileRep => _allTileRep;
    
    [SerializeField] private AllLevelsDataSO _allLevels;
    public AllLevelsDataSO AllLevels => _allLevels;
    
    [SerializeField] private int _currentLevel;
    public int CurrentLevel => _currentLevel;
    
    [SerializeField] private GenericKey _rocketDropKey;
    public GenericKey RocketDropKey => _rocketDropKey;

    [SerializeField] private GenericKey _hasShieldTag;
    public GenericKey HasShieldTag => _hasShieldTag;
    
    [SerializeField] private GenericKey _isSpawnBlockerTag;
    public GenericKey IsSpawnBlockerTag => _isSpawnBlockerTag;
    
    private Dictionary<Vector2Int, Actor> _cellDictionary = new Dictionary<Vector2Int, Actor>();
    public Dictionary<Vector2Int, Actor> CellDictionary
    {
        get => _cellDictionary;
        set => _cellDictionary = value;

    }
    private TileDropSwipeCommand _lastSwipeCommand;
    public TileDropSwipeCommand LastSwipeCommand
    {
        get => _lastSwipeCommand;
        set => _lastSwipeCommand = value;

    }
    private Actor _lastSwipedTileDrop;
    public Actor LastSwipedTileDrop
    {
        get => _lastSwipedTileDrop;
        set => _lastSwipedTileDrop = value;

    }
    private int _movingDropCount;
    public int MovingDropCount
    {
        get => _movingDropCount;
        set => _movingDropCount = value;
    }
    
    private List<Actor> _matchedDrops = new List<Actor>();
    public List<Actor> MatchedDrops
    {
        get => _matchedDrops;
        set => _matchedDrops = value;
    }

    private bool _hasMatch;
    public bool HasMatch
    {
        get => _hasMatch;
        set => _hasMatch = value;
    }
    
    private int _mustBeFilledCellCount;
    public int MustBeFilledCellCount
    {
        get => _mustBeFilledCellCount;
        set => _mustBeFilledCellCount = value;
    }

    public Actor GetCellActorWithCoordinates(Vector2Int position)
    {
        Actor cellActor;
        _cellDictionary.TryGetValue(position, out cellActor);
        return cellActor;
    }
    
    public Actor GetDropActorWithCoordinates(Vector2Int position)
    {
        Actor cellActor = GetCellActorWithCoordinates(position);
        Actor dropActor = cellActor ? cellActor.GetData<DS_TileCell>().OccupiedActor : null;
        return dropActor;
    }
    public Actor GetDropActorWithCoordinates(int x, int y)
    {
        Actor cellActor = GetCellActorWithCoordinates(new Vector2Int(x,y));
        Actor dropActor = cellActor.GetData<DS_TileCell>().OccupiedActor;
        return dropActor;
    }
    public GenericKey GetDropTypeKeyWithCoordinates(int x, int y)
    {
        Actor dropActor = GetDropActorWithCoordinates(new Vector2Int(x, y));
        return dropActor ? dropActor.GetData<DS_TileDrop>().DropTypeKey : null;
    }
    
    //
    public List<GenericKey> GetLevelDropTypes()
    {
        LevelDataSO levelData = AllLevels.Levels[CurrentLevel];
        return levelData.LevelDropTypeKeys;
    }

    public LevelDataSO GetCurrentLevelData()
    {
        LevelDataSO levelData = AllLevels.Levels[CurrentLevel];
        return levelData;
    }
   
}