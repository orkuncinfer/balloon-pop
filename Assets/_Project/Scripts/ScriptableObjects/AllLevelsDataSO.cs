using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/AllLevelsAsset", fileName = "AllLevelsAsset")]
public class AllLevelsDataSO : ScriptableObject
{
    public  List<LevelDataSO> Levels = new List<LevelDataSO>();
}