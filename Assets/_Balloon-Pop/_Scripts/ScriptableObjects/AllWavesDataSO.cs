using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/AllLevelsAsset", fileName = "AllLevelsAsset")]
public class AllWavesDataSO : ScriptableObject
{
    public  List<WaveDataSO> Levels = new List<WaveDataSO>();
}