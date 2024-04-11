using System.Collections;
using System.Collections.Generic;
using ECM2;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class State_ApplyJoystickInput : MonoState
{
    
    [SerializeField] private DataGetter<DS_Joystick> _joystickData;
    private CharacterInput _character;
    protected override void OnEnter()
    {
        base.OnEnter();
        _character = Owner.GetComponent<CharacterInput>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (_joystickData.Data == null)
        {
            _joystickData.GetData();
            return;
        }

        //Vector2 input = new Vector2(Input.GetAxis("Horizontal") + _joystickData.Data.Input.x,_joystickData.Data.Input.y );
        //_character.MoveInput = input;
    }
}
