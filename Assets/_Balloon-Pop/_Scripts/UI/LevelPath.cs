using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPath : MonoBehaviour
{
    public LevelNode[] Nodes;

    public int StartLevelNumber;
    public IntVar CurrentLevelIndexSO;

    public void UpdateLevelNodes()
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            bool locked = CurrentLevelIndexSO.Value + 1 < StartLevelNumber + i;
            Nodes[i].SetLocked(locked);
            Nodes[i].SetLevelNumber(StartLevelNumber + i);
        }
    }
}
