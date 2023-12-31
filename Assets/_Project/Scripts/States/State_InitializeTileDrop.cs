
public class State_InitializeTileDrop : MonoState
{
    private DS_TileDrop _dropData;
    protected override void OnEnter()
    {
        base.OnEnter();
        _dropData = Owner.GetData<DS_TileDrop>();

        if (_dropData.DropTypeKey != null)
        {
            _dropData.SpriteRenderer.sprite = _dropData.BoardData.AllTileRep.GetTileDropType(_dropData.DropTypeKey).Visual;
        }
        Owner.transform.name = _dropData.TileCoordinates + "_" + _dropData.DropTypeKey.ID;
        CheckoutExit();
    }
}