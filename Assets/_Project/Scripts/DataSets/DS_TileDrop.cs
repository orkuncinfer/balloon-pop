using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DS_TileDrop : Data
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    
    [SerializeField] private Animator _animator;
    public Animator Animator => _animator;
    
    [SerializeField][ReadOnly] private GenericKey _dropTypeKey;
    public GenericKey DropTypeKey  
    {
        get => _dropTypeKey;
        set => _dropTypeKey = value;
    }
    [SerializeField][ReadOnly] private bool _isMatched;
    public bool IsMatched  
    {
        get => _isMatched;
        set => _isMatched = value;
    }
    [SerializeField] [ReadOnly]private Actor _currentCell;
    public Actor CurrentCell  
    {
        get => _currentCell;
        set => _currentCell = value;
    }
    
    [SerializeField] private Vector2Int _tileCoordinates;
    public Vector2Int TileCoordinates  
    {
        get => _tileCoordinates;
        set => _tileCoordinates = value;
    }
    [SerializeField] private bool _isFalling;
    public bool IsFalling  
    {
        get => _isFalling;
        set => _isFalling = value;
    }
    
    [SerializeField] private bool _isSettled;
    public event Action<Actor, bool, bool> OnIsSettledChanged;
    public bool IsSettled
    {
        get => _isSettled;
        set
        {
            bool oldValue = _isSettled;
            bool isChanged = _isSettled != value;
            _isSettled = value;
            if (isChanged)
            {
                OnIsSettledChanged?.Invoke(Actor, oldValue, value);
            }
        }
    }
    
    private DS_TileBoard _boardData;
    public DS_TileBoard BoardData  
    {
        get => _boardData;
        set => _boardData = value;
    }
    
    [SerializeField]private float _gravity;
    public float Gravity  
    {
        get => _gravity;
        set => _gravity = value;
    }
    
    [SerializeField]private float _fallDelay;
    public float FallDelay  
    {
        get => _fallDelay;
        set => _fallDelay = value;
    }
}