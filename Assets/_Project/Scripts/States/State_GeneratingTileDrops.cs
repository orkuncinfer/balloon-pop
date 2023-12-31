using System.Collections.Generic;
using UnityEngine;

public class State_GeneratingTileDrops : MonoState
{
    private DS_TileBoard _boardData;
    private List<GenericKey> _tempPossibleTileDropTypes = new List<GenericKey>();
    
    protected override void OnEnter()
    {
        base.OnEnter();
        _boardData = Owner.GetData<DS_TileBoard>();
       
        GenerateTileDrops();
        
        CheckoutExit();
    }
    
    void GenerateTileDrops()
    {
        LevelDataSO levelData = _boardData.AllLevels.Levels[_boardData.CurrentLevel];
        for (int x = 0; x < levelData.BoardWidth; x++)
        {
            for (int y = 0; y < levelData.BoardHeight; y++)
            {
                GenericKey dropKey = levelData.BoardDropsDictionary.Get(new Vector2Int(x, y));
                GameObject dropPrefab = null;
                if (dropKey)
                {
                    dropPrefab = _boardData.AllTileRep.GetTileDropType(dropKey).Prefab;
                }
                else
                {
                    dropPrefab = _boardData.DropPrefab;
                }
                
                _tempPossibleTileDropTypes.Clear();
                _tempPossibleTileDropTypes.AddRange(levelData.LevelDropTypeKeys);
                
                if (x >= 2) // prevent initial matches horizontally
                {
                    Actor leftDrop = _boardData.GetCellActorWithCoordinates(new Vector2Int(x - 1, y)).GetData<DS_TileCell>().OccupiedActor;
                    Actor leftDrop2 = _boardData.GetCellActorWithCoordinates(new Vector2Int(x - 2, y)).GetData<DS_TileCell>().OccupiedActor;

                    DS_TileDrop leftDropData = leftDrop.GetData<DS_TileDrop>();
                    DS_TileDrop leftDropData2 = leftDrop2.GetData<DS_TileDrop>();
                    
                    if (leftDropData.DropTypeKey.ID == leftDropData2.DropTypeKey.ID || leftDropData2.DropTypeKey == _boardData.RocketDropKey)
                    {
                        _tempPossibleTileDropTypes.Remove(leftDropData.DropTypeKey);
                    }
                }
                
                if (y >= 2) // prevent initial matches vertically
                {
                    Actor upDrop = _boardData.GetCellActorWithCoordinates(new Vector2Int(x, y - 1)).GetData<DS_TileCell>().OccupiedActor;
                    Actor upDrop2 = _boardData.GetCellActorWithCoordinates(new Vector2Int(x , y -2)).GetData<DS_TileCell>().OccupiedActor;

                    DS_TileDrop upDropData = upDrop.GetData<DS_TileDrop>();
                    DS_TileDrop upDropData2 = upDrop2.GetData<DS_TileDrop>();
                    
                    if (upDropData.DropTypeKey.ID == upDropData2.DropTypeKey.ID || upDropData2.DropTypeKey == _boardData.RocketDropKey)
                    {
                        _tempPossibleTileDropTypes.Remove(upDropData.DropTypeKey);
                    }
                }
                int randomIndex;
                randomIndex = Random.Range(0, _tempPossibleTileDropTypes.Count);
                
                Vector2Int gridPosition = new Vector2Int(x, y);
                Actor cellActor = _boardData.GetCellActorWithCoordinates(new Vector2Int(x, y));
                
                GameObject dropInstance = GOPoolProvider.Retrieve(dropPrefab, Vector3.zero, Quaternion.identity, _boardData.TileHolder);
                Actor dropActor = dropInstance.GetComponent<Actor>();
                DS_TileDrop dropData = dropActor.GetData<DS_TileDrop>();

                if (dropKey)
                {
                    dropData.DropTypeKey = dropKey;
                }
                else
                {
                    dropData.DropTypeKey = _tempPossibleTileDropTypes[randomIndex];
                }
                dropData.CurrentCell = cellActor;
                dropData.BoardData = _boardData;
                dropData.TileCoordinates = new Vector2Int(x, y);
                dropActor.StartIfNot();
                cellActor.GetData<DS_TileCell>().OccupiedActor = dropActor;
                dropInstance.transform.SetParent(cellActor.transform);
                dropInstance.transform.localPosition = Vector3.zero;
                
            }
        }
    }
    
   
}