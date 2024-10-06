using System.Collections;
using System.Collections.Generic;
using BandoWare.GameplayTags;
using Sirenix.OdinInspector;
using UnityEngine;

public class TagContainerTest : MonoBehaviour
{
    [SerializeField] private GameplayTagContainer _container1;

    [SerializeField] private GameplayTagContainer _container2;
    
    [Button]
    public void Test()
    {
        _container2.AddTags(_container1);
    }
    [Button]
    public void Test2()
    {
        //_container2.HasTag(_container1);
    }
}
