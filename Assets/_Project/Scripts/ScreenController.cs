using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public List<Panel> PanelList;
}

[Serializable]
public class Panel
{
    public GameObject Reference;
    public string Key;

    [Button]
    public void MyButton()
    {
        
    }
}
