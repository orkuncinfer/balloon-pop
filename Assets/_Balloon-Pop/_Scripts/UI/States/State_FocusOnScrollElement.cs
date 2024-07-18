using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_FocusOnScrollElement : MonoState
{
    [SerializeField] private ScrollRectEnsureVisible _scrollRectEnsureVisible;
    private DS_PlayerPersistent _playerPersistent;
    private DS_LevelSelection _levelSelection;
    protected override void OnEnter()
    {
        base.OnEnter();
        _playerPersistent = Owner.GetData<DS_PlayerPersistent>();
        _levelSelection = Owner.GetData<DS_LevelSelection>();
        StartCoroutine(FrameDelayCoroutine());
    }
    
    IEnumerator FrameDelayCoroutine()
    {
        yield return null;
        RectTransform target = _levelSelection.LevelNodes[_playerPersistent.CurrentLevelIndex].GetComponent<RectTransform>();
        _scrollRectEnsureVisible.FocusOnRectTween(target,0, new Vector2(0,200f));
    }
}
