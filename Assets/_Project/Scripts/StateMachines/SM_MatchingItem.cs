using UnityEngine;

public class SM_MatchingItem : ActorStateMachine
{
    protected override MonoState _initialState => _initializeState;

    [SerializeField] private MonoState _initializeState;
    [SerializeField] private MonoState _idleState;
    [SerializeField] private MonoState _movingToDesiredCell;
    [SerializeField] private MonoState _matchedState;

    private DS_TileDrop _dropData;

    public override void OnInitialize()
    {
        base.OnInitialize();
        _dropData = Owner.GetData<DS_TileDrop>();
    }

    public override void OnRequireAddTransitions()
    {
        AddTransition(_initializeState,_idleState,InitializeToIdleCondition);
        AddTransition(_idleState,_movingToDesiredCell, IdleToMovingCondition);
        AddTransition(_movingToDesiredCell,_idleState, MovingToIdleCondition);
        AddTransition(_idleState,_matchedState, IdleToMatched);
    }

    private bool IdleToMatched()
    {
        return _dropData.IsMatched;
    }

    private bool MovingToIdleCondition()
    {
        return Owner.transform.position == _dropData.CurrentCell.transform.position;
    }

    private bool IdleToMovingCondition()
    {
        return Owner.transform.position != _dropData.CurrentCell.transform.position;
    }

    private bool InitializeToIdleCondition()
    {
        return _initializeState.IsFinished;
    }
}