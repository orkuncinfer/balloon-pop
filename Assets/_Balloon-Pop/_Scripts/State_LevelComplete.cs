using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_LevelComplete : MonoState
{
    DS_PlayerRuntime _playerRuntime;
    DS_PlayerPersistent _playerPersistent;
    DS_GameModePersistent _gameModePersistent;
    protected override void OnEnter()
    {
        base.OnEnter();
        _playerRuntime = ActorRegistry.PlayerActor.GetData<DS_PlayerRuntime>();
        _playerPersistent = Owner.GetData<DS_PlayerPersistent>();
        _gameModePersistent = Owner.GetData<DS_GameModePersistent>();
        
        int currentLevelIndex = _gameModePersistent.CurrentLevelIndex;
        
        float healthPercentage = _playerRuntime.CurrentHealth / (float)_playerPersistent.MaxHealth;
        
        _playerPersistent.LevelCompletionProgress[currentLevelIndex] = healthPercentage;
        
        _gameModePersistent.CurrentLevelIndex++;
        if(_gameModePersistent.CurrentLevelIndex > _gameModePersistent.MaxReachedLevelIndex)
        {
            _gameModePersistent.MaxReachedLevelIndex = _gameModePersistent.CurrentLevelIndex;
        }
    }

    protected override void OnExit()
    {
        base.OnExit();

        Owner.GetData<DS_GameModeRuntime>().ResetResult();
    }
}
