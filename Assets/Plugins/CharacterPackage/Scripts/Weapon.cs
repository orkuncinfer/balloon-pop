﻿using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] private string m_Id;
    public string id => m_Id;
    
    [SerializeField]
    private Actor _owner; 
    public Actor Owner 
    {
        get => _owner;
        set => _owner = value;
    }

    private void Reset()
    {
        m_Id = gameObject.name;
    }
}