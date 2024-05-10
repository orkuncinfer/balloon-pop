using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_RefVar : Data
{
    [SerializeField]
    private Object _reference; 
    public Object Reference 
    {
        get => _reference;
        set => _reference = value;
    }
}
