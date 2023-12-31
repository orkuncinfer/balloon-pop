using UnityEngine;

public class State_TileCellEmpty : MonoState
{
    private DS_TileCell _cellData;
    private DS_TileBoard _boardData;
    private bool _added;
    protected override void OnEnter()
    {
        base.OnEnter();
        _cellData = Owner.GetData<DS_TileCell>();
        _boardData = _cellData.BoardData;

        Actor cellOnTop = _boardData.GetCellActorWithCoordinates(new Vector2Int(_cellData.TileCoordinates.x,_boardData.Height - 1));
        if(cellOnTop == null) return;
        if (!cellOnTop.ContainsTag(_boardData.IsSpawnBlockerTag.ID))
        {
            _cellData.BoardData.MustBeFilledCellCount++;
            _added = true;
        }
    }
    protected override void OnExit()
    {
        base.OnExit();
        
        Actor cellOnTop = _boardData.GetCellActorWithCoordinates(new Vector2Int(_cellData.TileCoordinates.x,_boardData.Height - 1));
        if(cellOnTop == null) return;
        if (!cellOnTop.ContainsTag(_boardData.IsSpawnBlockerTag.ID) && _added)
        {
            _cellData.BoardData.MustBeFilledCellCount--;
            _added = false;
        }
    }
}