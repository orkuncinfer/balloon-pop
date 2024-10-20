using System;
using System.Collections;
using System.Collections.Generic;
using BandoWare.GameplayTags;
using Sirenix.OdinInspector;
using UnityEngine;

public class TagContainerTest : MonoBehaviour
{
    [SerializeField] private GameplayTagContainer _container1;

    [SerializeField] private GameplayTagContainer _container2;

    private void Awake()
    {
        _container1.OnTagChanged += OnTagChanged;
        _container2.OnTagChanged += OnTagChanged;
    }

    private void OnTagChanged()
    {
        Debug.Log($"Ability.Crouch");
    }

    [Button]
    public void Test()
    {
        _container2.AddTags(_container1);
    }
    [Button]
    public void Test2(string tagName)
    {
        BandoWare.GameplayTags.GameplayTag newTag = GameplayTagManager.RequestTag(tagName);
        _container2.AddTag(newTag);
    }
}
