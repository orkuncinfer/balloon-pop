using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_BPGameModeFinished : MonoState
{
    private DS_PlayerPersistent _playerPersistent;
    protected override void OnEnter()
    {
        base.OnEnter();
        _playerPersistent = Owner.GetData<DS_PlayerPersistent>();
        
        if (_playerPersistent.HasOngoingSession)
        {
            _playerPersistent.HasOngoingSession = false;
        }
    }
}
