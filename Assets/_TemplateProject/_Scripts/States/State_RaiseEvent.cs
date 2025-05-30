using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_RaiseEvent : MonoState
{
    [SerializeField] private EventField _eventField;

    protected override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("State_RaiseEvent OnEnter");
        _eventField.Raise(Owner);
    }
}
