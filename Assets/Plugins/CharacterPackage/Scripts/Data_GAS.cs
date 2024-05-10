using System.Collections;
using System.Collections.Generic;
using Core.Editor;
using StatSystem;
using UnityEngine;

public class Data_GAS : Data
{
    [SerializeField]
    private StatController _statController; 
    public StatController StatController 
    {
        get => _statController;
        set => _statController = value;
    }
    
    [SerializeField]
    private AbilityController _abilityController; 
    public AbilityController AbilityController 
    {
        get => _abilityController;
        set => _abilityController = value;
    }
    
    [SerializeField]
    private GameplayEffectController _effectController; 
    public GameplayEffectController EffectController 
    {
        get => _effectController;
        set => _effectController = value;
    }
    
    [SerializeField]
    private LevelController _levelController; 
    public LevelController LevelController 
    {
        get => _levelController;
        set => _levelController = value;
    }
    
    [SerializeField]
    private TagController _tagController; 
    public TagController TagController 
    {
        get => _tagController;
        set => _tagController = value;
    }
}
