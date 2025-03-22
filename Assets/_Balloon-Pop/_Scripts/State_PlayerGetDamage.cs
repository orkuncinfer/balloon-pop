using System.Collections;
using System.Collections.Generic;
using Heimdallr.Core;
using MoreMountains.Feedbacks;
using UnityEngine;

public class State_PlayerGetDamage : MonoState
{
    [SerializeField] private EventField<GameObject> _onBalloonDied;
    private DS_PlayerRuntime _playerRuntime;
    [SerializeField] private TriggerDetector2D _detector2D;
    [SerializeField] private MMF_Player _damageEffect;
    protected override void OnEnter()
    {
        base.OnEnter();
        _playerRuntime = Owner.GetData<DS_PlayerRuntime>();
        _detector2D.onTriggerEnter += OnTrigger;
    }

    protected override void OnExit()
    {
        base.OnExit();
        _detector2D.onTriggerEnter -= OnTrigger;
    }

    private void OnTrigger(Collider2D arg0)
    {
        if(arg0.TryGetComponent(out Balloon balloon))
        {
            int damage = balloon.ItemDefinition.GetData<DataVar_Int>(TagEnum.Damage.ToString()).Value;
            _playerRuntime.CurrentHealth -= damage;
            _damageEffect.PlayFeedbacks();
            if (_playerRuntime.CurrentHealth <= 0)
            {
                _playerRuntime.CurrentHealth = 0;
            }
            PoolManager.ReleaseObject(balloon.gameObject);
            _onBalloonDied.Raise(balloon.gameObject);
        }
    }
}
