using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_MonsterKilled : MonoState
{
    private Data_Monster _monsterData;
    protected override void OnEnter()
    {
        base.OnEnter();
        _monsterData = Owner.GetData<Data_Monster>();
        
        Events.onExampleEvent.Invoke(new MobKilledEventArgs()
        {
            MobId = _monsterData.Definition.MonsterID,
            Position = Owner.transform.position+Utils.GetCenterOfCollider(Owner.transform)
        });
    }
}
