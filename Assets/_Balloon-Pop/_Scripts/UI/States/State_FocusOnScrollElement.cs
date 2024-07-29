using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_FocusOnScrollElement : MonoState
{
    [SerializeField] private ScrollRectEnsureVisible _scrollRectEnsureVisible;
    private DS_GameModePersistent _gamemodePersistent;
    private DS_LevelSelection _levelSelection;
    protected override void OnEnter()
    {
        base.OnEnter();
        _gamemodePersistent = Owner.GetData<DS_GameModePersistent>();
        _levelSelection = Owner.GetData<DS_LevelSelection>();
        StartCoroutine(FrameDelayCoroutine());
    }
    
    IEnumerator FrameDelayCoroutine()
    {
        yield return null;
        RectTransform target = _levelSelection.LevelNodes[_gamemodePersistent.CurrentLevelIndex].GetComponent<RectTransform>();
        _scrollRectEnsureVisible.FocusOnRectTween(target,0, new Vector2(0,200f));
    }
}
