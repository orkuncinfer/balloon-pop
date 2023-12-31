using UnityEngine;

public class SM_TileCell : ActorStateMachine
{
    protected override MonoState _initialState => _emptyState;

    [SerializeField] private MonoState _occupiedState;
    [SerializeField] private MonoState _emptyState;
    
    private DS_TileCell _cellData;

    protected override void OnEnter()
    {
        base.OnEnter();
        _cellData = Owner.GetData<DS_TileCell>();
    }

    public override void OnRequireAddTransitions()
    {
        AddTransition(_emptyState,_occupiedState,EmptyToOccupiedCondition);
        AddTransition(_occupiedState,_emptyState,OccupiedToEmptyCondition);
    }

    private bool EmptyToOccupiedCondition()
    {
        return _cellData.OccupiedActor;
    }

    private bool OccupiedToEmptyCondition()
    {
        return !_cellData.OccupiedActor;
    }
}