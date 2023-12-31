using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class State_GeneratingTileCells : MonoState
{
    private DS_TileBoard _boardData;
    
    private List<GenericKey> _tempPossibleTileDropTypes = new List<GenericKey>();
    
    protected override void OnEnter()
    {
        base.OnEnter();
        _boardData = Owner.GetData<DS_TileBoard>();
        
        CenterBoardHolder();
        GenerateGrid();
        ScaleBoardHolderAccordingToScreenSize();
        
        CheckoutExit();
    }
    
    void GenerateGrid()
    {
        LevelDataSO levelData = _boardData.AllLevels.Levels[_boardData.CurrentLevel];
        for (int x = 0; x < levelData.BoardWidth; x++)
        {
            for (int y = 0; y < levelData.BoardHeight; y++)
            {
                GenericKey cellKey = levelData.BoardCellsDictionary.Get(new Vector2Int(x, y));
                GameObject cellPrefab = _boardData.AllTileRep.GetTileCellType(cellKey).Prefab;
                
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition = new Vector3(x * _boardData.Spacing, y * _boardData.Spacing, 0);
                
                GameObject cellInstance = Instantiate(cellPrefab, worldPosition, Quaternion.identity, _boardData.TileHolder);
                Actor cellActor = cellInstance.GetComponent<Actor>();
                cellActor.GetData<DS_TileCell>().TileCoordinates = new Vector2Int(x, y);
                cellActor.GetData<DS_TileCell>().BoardData = _boardData;
                cellActor.StartIfNot();
               
                cellInstance.name = "Cell_" + x + "_" + y;

                _boardData.CellDictionary[gridPosition] = cellActor;
            }
        }
    }
    void ScaleBoardHolderAccordingToScreenSize()
    {
        float boardPixelWidth = _boardData.Width + (_boardData.Width - 1) * _boardData.Spacing / 10;
        _boardData.TileHolder.position = Vector3.zero;
        _boardData.TileHolder.localScale = Vector3.one * (GetScreenToWorldWidth / boardPixelWidth) * _boardData.FillScreenPercentage;
    }

    void CenterBoardHolder()
    {
        float boardPixelWidth = _boardData.Width + (_boardData.Width - 1) * _boardData.Spacing / 10;
        float boardPixelHeight = _boardData.Height + (_boardData.Height - 1) * _boardData.Spacing / 10;
        Vector2 centerPos = new Vector2(boardPixelWidth / 2, boardPixelHeight / 2);
        _boardData.TileHolder.SetParent(null);
        _boardData.TileHolder.position = centerPos;
    }
    
    public static float GetScreenToWorldWidth
    {
        get
        {
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
            var width = edgeVector.x * 2;
            return width;
        }
    }
}