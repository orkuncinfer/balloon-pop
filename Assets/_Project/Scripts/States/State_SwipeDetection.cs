using UnityEngine;
using UnityEngine.Serialization;

public class State_SwipeDetection : MonoState
{
    [SerializeField] private EventSignal _checkForMatchEvent;
 
    private Actor _draggedTileDrop;
    private Actor _targetTileDrop;
    private DS_TileBoard _boardData;
    private DS_TileDrop _draggedTileDropData;
    
    private Vector2 _startTouchPosition;
    private Vector2 _currentTouchPosition;
    public float SwipeThreshold;
    private TileDropSwipeCommandPool _commandPool = new TileDropSwipeCommandPool();
    
    private bool _isSwiping;
    
    protected override void OnEnter()
    {
        base.OnEnter();
        _boardData = Owner.GetData<DS_TileBoard>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        
        if (_boardData.MovingDropCount > 0) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider == null) return;
            if (hit.transform.TryGetComponent(out Actor hitActor))
            {
                if (hitActor == null) return;
                _draggedTileDropData = hitActor.GetData<DS_TileDrop>();
                _draggedTileDrop = _draggedTileDropData.Actor;
            }
            
            _isSwiping = true;
            _startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && _isSwiping)
        {
            _currentTouchPosition = (Vector2)Input.mousePosition;
            if (Vector2.Distance(_startTouchPosition, _currentTouchPosition) > SwipeThreshold)
            {
                _isSwiping = false;
                DetectSwipeDirection();
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            _draggedTileDrop = null;
            _targetTileDrop = null;
        }
    }
    
    void DetectSwipeDirection()
    {
        if (_draggedTileDropData == null)
        {
            return;
        }
        
        float horizontalDistance = _currentTouchPosition.x - _startTouchPosition.x;
        float verticalDistance = _currentTouchPosition.y - _startTouchPosition.y;

        if (Mathf.Abs(horizontalDistance) > Mathf.Abs(verticalDistance))
        {
            if (horizontalDistance > 0)// swiped right
            {
                if (_draggedTileDropData.CurrentCell != null)
                {
                    if (_draggedTileDropData.TileCoordinates.x >= _boardData.Width - 1) return;
                    SwipeDrops(_draggedTileDropData.TileCoordinates + new Vector2Int(1,0));
                }
            }
            else // swiped left
            {
                if (_draggedTileDropData.CurrentCell != null)
                {
                    if(_draggedTileDropData.TileCoordinates.x == 0) return;
                    SwipeDrops(_draggedTileDropData.TileCoordinates + new Vector2Int(-1,0));
                }
            }
        }
        else
        {
            if (verticalDistance > 0) // swiped up
            {
                if (_draggedTileDropData.CurrentCell != null)
                {
                    if(_draggedTileDropData.TileCoordinates.y >= _boardData.Height - 1) return;
                    SwipeDrops(_draggedTileDropData.TileCoordinates + new Vector2Int(0,1));
                }
            }
            else // swiped down
            {
                if(_draggedTileDropData.TileCoordinates.y == 0) return;
                SwipeDrops(_draggedTileDropData.TileCoordinates + new Vector2Int(0,-1));
            }
        }
    }
    
    void SwipeDrops(Vector2Int targetDropCoordinates)
    {
        Actor targetCell = _boardData.GetCellActorWithCoordinates(targetDropCoordinates);
        Actor draggedCell = _boardData.GetCellActorWithCoordinates(_draggedTileDropData.TileCoordinates);

        if (draggedCell.ContainsTag(_boardData.HasShieldTag.ID) || targetCell.ContainsTag(_boardData.HasShieldTag.ID))
        {
            return;
        }
        
        if (targetCell.GetData<DS_TileCell>().OccupiedActor == null ||
            draggedCell.GetData<DS_TileCell>().OccupiedActor == null)
        {
            return; // we are trying to drag to a null target
        }
        Actor targetDrop = targetCell.GetData<DS_TileCell>().OccupiedActor;
        Actor draggedDrop = draggedCell.GetData<DS_TileCell>().OccupiedActor;
        
        
        TileDropSwipeCommand command = _commandPool.Get(_boardData, draggedDrop, targetDrop);
        command.Execute();

        _boardData.LastSwipeCommand = command;
        _boardData.LastSwipedTileDrop = draggedDrop;

        _draggedTileDropData.OnIsSettledChanged += OnDraggedDropSettle;
        
        _checkForMatchEvent.Raise();
    }

    private void OnDraggedDropSettle(Actor arg1, bool arg2, bool arg3)
    {
        if (arg3 == true)
        {
            _draggedTileDropData.OnIsSettledChanged -= OnDraggedDropSettle;
        }
    }
}