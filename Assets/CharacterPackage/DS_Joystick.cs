using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobileInput;
using Sirenix.OdinInspector;
using UnityEngine;

public class DS_Joystick : Data
{
    public Joystick Joystick;
    public Vector2 Input;
    
    private void Update()
    {
        Input = Joystick.CurrentProcessedValue;
    }
}
