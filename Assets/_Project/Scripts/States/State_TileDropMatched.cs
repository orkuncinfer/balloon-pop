using UnityEngine;

public class State_TileDropMatched : MonoState
{
    private DS_TileDrop _dropData;
    [SerializeField] private GameObject _visual;

    protected override void OnEnter()
    {
        base.OnEnter();
        _dropData = Owner.GetData<DS_TileDrop>();
        GameObject matchedEffect = _dropData.BoardData.AllTileRep.TileDropTypesDict[_dropData.DropTypeKey]
            .MatchedEffectPrefab;
        if (matchedEffect)
        {
            GOPoolProvider.Retrieve(matchedEffect,_visual.transform.position, Quaternion.identity);
        }
        
        ResetVariables();
        Owner.StopIfNot();
    }

    void ResetVariables()
    {
        _dropData.CurrentCell.GetData<DS_TileCell>().OccupiedActor = null;
        _dropData.CurrentCell = null;
        _dropData.IsFalling = false;
        _dropData.IsSettled = false;
        _dropData.IsMatched = false;
        _dropData.DropTypeKey = null;
    }
}