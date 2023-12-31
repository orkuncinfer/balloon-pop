using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_MovingToCurrentCell : MonoState
{
    private DS_TileDrop _dropData;
    [SerializeField] private EventSignal _checkForMatchEvent;
    
    private float _startTime;
    protected override void OnEnter()
    {
        base.OnEnter();
        _dropData = Owner.GetData<DS_TileDrop>();

        //_dropData.Animator.SetTrigger("Falling");
        _dropData.BoardData.MovingDropCount++;
        _dropData.IsSettled = false;
        _startTime = Time.time;
        Debug.Log("delay is " + _dropData.FallDelay);
    }
    
    protected override void OnExit()
    {
        base.OnExit();
        _dropData.IsSettled = true;
        Owner.transform.SetParent(_dropData.CurrentCell.transform);
        _dropData.BoardData.MovingDropCount--;
        _dropData.IsFalling = false;
        _dropData.FallDelay = 0;
        _dropData.Animator.SetTrigger("Settled");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time > _startTime + _dropData.FallDelay)
        {
            Owner.transform.position = Vector2.MoveTowards(Owner.transform.position,_dropData.CurrentCell.transform.position, Time.deltaTime * _dropData.Gravity);
        }
        
    }
}