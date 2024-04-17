using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Data_Camera : Data
{
    [SerializeField] private CinemachineVirtualCamera _currentCamera;
    public CinemachineVirtualCamera CurrentCamera => _currentCamera;
}
