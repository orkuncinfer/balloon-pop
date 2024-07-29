using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelPath : MonoBehaviour
{
    public LevelNode[] Nodes;

    public int StartLevelNumber;

    [Button]
    public void UpdateLevelNodes(int currentLevelIndex)
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            bool locked = currentLevelIndex + 1 < StartLevelNumber + i;
            Nodes[i].SetLocked(locked);
            Nodes[i].SetLevelNumber(StartLevelNumber + i);
        }
    }
}
