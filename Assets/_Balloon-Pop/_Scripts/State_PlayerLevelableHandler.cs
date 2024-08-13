using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_PlayerLevelableHandler : MonoState

{
    private DS_PlayerRuntime _playerPersistent;

    protected override void OnEnter()
    {
        base.OnEnter();
        _playerPersistent = Owner.GetData<DS_PlayerRuntime>();
        _playerPersistent.RuntimePlayerLevel.ResetLevel();
        _playerPersistent.RuntimePlayerLevel.onLevelUp += OnLevelUp;
    }
    protected override  void OnExit()
    {
        base.OnExit();
        _playerPersistent.RuntimePlayerLevel.onLevelUp -= OnLevelUp;
    }

    private void OnLevelUp(int obj)
    {
        
    }
}
