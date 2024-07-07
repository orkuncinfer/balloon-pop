using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ActorStarterOnEvent : MonoBehaviour
{
    [SerializeField] private Actor _actor;
    [SerializeField] private EventField _eventField;

    private void Awake()
    {
        _eventField.Register(null, OnEventRaised);
    }

    private void OnEventRaised(EventArgs obj)
    {
        _actor.StartIfNot();
    }

    [Button]
    public void Test()
    {
        Resources.UnloadUnusedAssets();
    }
}
