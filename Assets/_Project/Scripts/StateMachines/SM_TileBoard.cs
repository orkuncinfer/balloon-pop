using UnityEngine;

public class SM_TileBoard : ActorStateMachine
{
    protected override MonoState _initialState => _generatingCellsState;

    [SerializeField] private MonoState _generatingCellsState;
    [SerializeField] private MonoState _generatingDropsState;
    [SerializeField] private MonoState _playingState;
    [SerializeField] private MonoState _matchHappenedState;
    [SerializeField] private MonoState _applyingGravityState;

    private DS_TileBoard _boardData;

    protected override void OnEnter()
    {
        base.OnEnter();
        _boardData = Owner.GetData<DS_TileBoard>();
    }

    public override void OnRequireAddTransitions()
    {
        AddTransition(_generatingCellsState,_generatingDropsState,GeneratingCellsToDropsCondition);
        AddTransition(_generatingDropsState,_playingState, GeneratingDropsToPlayingCondition);
        AddTransition(_playingState,_matchHappenedState, PlayingToMatchedCondition);
        AddTransition(_matchHappenedState,_applyingGravityState, MatchedToGravityState);
        AddTransition(_applyingGravityState,_playingState, GravityToPlayingCondition);
        AddTransition(_playingState,_applyingGravityState, PlayingToGravityCondition);
    }

    private bool PlayingToGravityCondition()
    {
        return _boardData.MustBeFilledCellCount > 0;
    }

    private bool GravityToPlayingCondition()
    {
        return !_applyingGravityState.IsRunning;
    }

    private bool MatchedToGravityState()
    {
        return !_matchHappenedState.IsRunning;
    }
    
    private bool PlayingToMatchedCondition()
    {
        return _boardData.HasMatch || _boardData.MatchedDrops.Count > 0;
    }

    private bool GeneratingCellsToDropsCondition()
    {
        return _generatingCellsState.IsFinished;
    }

    private bool GeneratingDropsToPlayingCondition()
    {
        return _generatingDropsState.IsFinished;
    }
    
}