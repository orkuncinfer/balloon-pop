using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class State_ApplyGravityOnDrops : MonoState
{
    private DS_TileBoard _boardData;
    private List<DS_TileDrop> _fallingDropDataList = new List<DS_TileDrop>();

    private int _totalEmptyCount;
    private int _firstEmptyCellIndex;
    
    protected override void OnEnter()
    {
        base.OnEnter();
        _boardData = Owner.GetData<DS_TileBoard>();
        _fallingDropDataList.Clear();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        int movingCount = 0;
        _totalEmptyCount = -1;
        _firstEmptyCellIndex = -1;
        for (int x = 0; x < _boardData.Width; x++)
        {
            for (int y = 0; y < _boardData.Height - 1; y++)
            {
                Actor currentCell = _boardData.GetCellActorWithCoordinates(new Vector2Int(x, y));
                DS_TileCell currentCellData = currentCell.GetData<DS_TileCell>();
                

                if (currentCellData.OccupiedActor == null)
                {
                    if (_totalEmptyCount == -1)
                    {
                        _totalEmptyCount = EmptyCountOnColumn(x);
                        //Debug.Log("totalempty : " + _totalEmptyCount);
                    }
                    if (_firstEmptyCellIndex == -1)
                    {
                        _firstEmptyCellIndex = y;
                        //Debug.Log("first empty : " + _firstEmptyCellIndex);
                    }
                    
                    
                    for (int aboveY = y + 1; aboveY < _boardData.Height; aboveY++)
                    {
                        Actor aboveCell = _boardData.GetCellActorWithCoordinates(new Vector2Int(x, aboveY));
                        DS_TileCell aboveCellData = aboveCell.GetData<DS_TileCell>();
                        
                        

                        if (aboveCellData.OccupiedActor != null) // if the cell above has a drop
                        {
                            DS_TileDrop aboveDrop = aboveCellData.OccupiedActor.GetData<DS_TileDrop>();

                            int distanceToFall = aboveY - _firstEmptyCellIndex;

                            aboveDrop.FallDelay = Mathf.Abs(_totalEmptyCount - distanceToFall) * 0.08f;
                            Debug.Log("fall Delay : " + aboveDrop.FallDelay);
                            aboveDrop.CurrentCell = currentCell;
                            aboveDrop.TileCoordinates = new Vector2Int(x, y);
                            _fallingDropDataList.Add(aboveDrop);
                            aboveDrop.IsFalling = true;
                            
                            currentCellData.OccupiedActor = aboveDrop.Actor;
                            aboveCellData.OccupiedActor = null;
                            _boardData.LastSwipedTileDrop = aboveDrop.Actor;
                         
                            break;
                        }
                    }
                }
            }

            _totalEmptyCount = _totalEmptyCount * 2;
            
            int firstEmptyYIndex = -1;
            // if first cell is not spawner, continue to next column
            Actor cellOnTop = _boardData.GetCellActorWithCoordinates(new Vector2Int(x, _boardData.Height - 1));
            if(cellOnTop.ContainsTag(_boardData.IsSpawnBlockerTag.ID)) continue;
            
            for (int y =0; y < _boardData.Height; y++)
            {
                Actor currentCell = _boardData.GetCellActorWithCoordinates(new Vector2Int(x, y));
                DS_TileCell currentCellData = currentCell.GetData<DS_TileCell>();
                if (currentCellData.OccupiedActor == null)
                {
                    GameObject dropInstance = GOPoolProvider.Retrieve(_boardData.DropPrefab, Vector3.zero, Quaternion.identity, _boardData.TileHolder);
                    Actor dropActor = dropInstance.GetComponent<Actor>();
                    DS_TileDrop dropData = dropActor.GetData<DS_TileDrop>();
                    if (firstEmptyYIndex == -1)
                    {
                        firstEmptyYIndex = y;
                    }
                    
                    int randomIndex;
                    randomIndex = Random.Range(0, _boardData.GetCurrentLevelData().LevelDropTypeKeys.Count);
                    dropData.DropTypeKey = _boardData.GetCurrentLevelData().LevelDropTypeKeys[randomIndex];
                    dropData.CurrentCell = currentCell;
                    dropData.BoardData = _boardData;
                    dropData.TileCoordinates = new Vector2Int(x, y);

                    int newY = y + _totalEmptyCount;
                    int distanceToFall = newY - _firstEmptyCellIndex;
                    //Debug.Log("heighttt" + distanceToFall +"=" + newY +"-" + _firstEmptyCellIndex );
                    dropData.FallDelay = Mathf.Abs(_totalEmptyCount - distanceToFall) * 0.08f;
                    Debug.Log("fall Delay : " + dropData.FallDelay);
                    dropActor.StartIfNot();
             
                    currentCell.GetData<DS_TileCell>().OccupiedActor = dropActor;
                    dropInstance.transform.SetParent(currentCell.transform);
                    Vector2 desiredPos = new Vector2(0, (_boardData.Height - firstEmptyYIndex) * _boardData.Spacing);
                    dropInstance.transform.localPosition = desiredPos;


                }
            }
            
            _totalEmptyCount = -1;
            _firstEmptyCellIndex = -1;
        }

        int fallingCount = 0; // wait for all drops to fall
        for (int i = 0; i < _fallingDropDataList.Count; i++)
        {
            if (_fallingDropDataList[i].IsFalling)
            {
                fallingCount++;
            }
        }
        
        if (fallingCount == 0)
        {
            CheckoutExit();
        }
    }
    
    
    int EmptyCountOnColumn(int columnIndex)
    {
        int count = 0;
            
        for (int y = 0; y < _boardData.Height - 1; y++)
        {
            Actor currentCell = _boardData.GetCellActorWithCoordinates(new Vector2Int(columnIndex, y));
            DS_TileCell currentCellData = currentCell.GetData<DS_TileCell>();

            if (currentCellData.OccupiedActor == null)
            {
                count++;
            }
        }
            
        return count;
    }
}