using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SRDebuggerActivator : MonoBehaviour
{
    [SerializeField] private InputAction _toggleAction;

    private void Start()
    {
        _toggleAction.Enable();
        _toggleAction.performed += OnPerformed;
    }

    private void OnPerformed(InputAction.CallbackContext obj)
    {
        SRDebug.Init();
        Debug.Log("SRDebugger Activated");
    }
}
