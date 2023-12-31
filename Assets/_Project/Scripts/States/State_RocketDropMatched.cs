using UnityEngine;

public class State_RocketDropMatched : MonoState
{
    private DS_TileDrop _dropData;
    private DS_TileBoard _boardData;
    [SerializeField] private GameObject _visual;

    protected override void OnEnter()
    {
        base.OnEnter();
        _dropData = Owner.GetData<DS_TileDrop>();
        _boardData = _dropData.BoardData;
        
        _dropData.CurrentCell.GetData<DS_TileCell>().OccupiedActor = null;
        _dropData.CurrentCell = null;
        
        int width = _boardData.AllLevels.Levels[_boardData.CurrentLevel].BoardWidth;
        int y = _dropData.TileCoordinates.y;
        for (int x = 0; x < width; x++)
        {
          Actor dropActor = _boardData.GetDropActorWithCoordinates(new Vector2Int(x, y));
          if (dropActor)
          {
              DS_TileDrop dropData= dropActor.GetData<DS_TileDrop>();
              dropData.IsMatched = true;

              if (dropData.CurrentCell != null)
              {
                  if (dropData.CurrentCell.ContainsTag(_boardData.HasShieldTag.ID))
                  {
                      dropData.CurrentCell.RemoveTag(_boardData.HasShieldTag.ID);
                  }
              }
          }
        }
        GameObject matchedEffect = _dropData.BoardData.AllTileRep.TileDropTypesDict[_dropData.DropTypeKey]
            .MatchedEffectPrefab;
        if (matchedEffect)
        {
            GOPoolProvider.Retrieve(matchedEffect,_visual.transform.position, Quaternion.identity);
        }
        
        Owner.StopIfNot();
    }
}