using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDropSwipeCommand
{
    private DS_TileBoard boardActor;
    private Actor _targetDrop;
    private Actor _draggedDrop;
    private TileDropSwipeCommandPool _ownerPool;

    public TileDropSwipeCommand(DS_TileBoard board, Actor draggedDrop, Actor targetDrop, TileDropSwipeCommandPool ownerPool)
    {
        boardActor = board;
        _targetDrop = targetDrop;
        _draggedDrop = draggedDrop;
        _ownerPool = ownerPool;
    }
    public void Reset(DS_TileBoard board, Actor draggedDrop, Actor targetDrop)
    {
        boardActor = board;
        _targetDrop = targetDrop;
        _draggedDrop = draggedDrop;
    }
    private void Swap(Actor draggedDrop, Actor targetDrop)
    {
        DS_TileDrop draggedDropData = draggedDrop.GetData<DS_TileDrop>();
        DS_TileDrop targetDroppData = targetDrop.GetData<DS_TileDrop>();
        
        Actor targetCell = boardActor.GetCellActorWithCoordinates(targetDroppData.TileCoordinates);
        Actor draggedCell = boardActor.GetCellActorWithCoordinates(draggedDropData.TileCoordinates);
        
        DS_TileCell targetCellData = targetCell.GetData<DS_TileCell>();
        DS_TileCell draggedCellData = draggedCell.GetData<DS_TileCell>();
        
        targetCellData.OccupiedActor = draggedDrop;
        draggedCellData.OccupiedActor = targetDrop;

        targetDroppData.CurrentCell = draggedDropData.CurrentCell;
        draggedDropData.CurrentCell = targetCell;

        (targetDroppData.TileCoordinates,draggedDropData.TileCoordinates) =  (draggedDropData.TileCoordinates,targetDroppData.TileCoordinates);
    }
    public void Execute()
    {
        Swap(_draggedDrop,_targetDrop);
    }

    public void Undo()
    {
        Swap(_targetDrop,_draggedDrop);
    }

    public void ReturnToPool()
    {
        _ownerPool.Return(this);
    }
}
public class TileDropSwipeCommandPool
{
    private Stack<TileDropSwipeCommand> pool = new Stack<TileDropSwipeCommand>();

    public TileDropSwipeCommand Get(DS_TileBoard board, Actor targetDrop, Actor draggedDrop)
    {
        if (pool.Count > 0)
        {
            var command = pool.Pop();
            command.Reset(board, targetDrop, draggedDrop);
            return command;
        }
        else
        {
            return new TileDropSwipeCommand(board, targetDrop, draggedDrop, this);
        }
    }

    public void Return(TileDropSwipeCommand command)
    {
        pool.Push(command);
    }
}
