using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelPath : MonoBehaviour
{
    public LevelNode[] Nodes;

    public int StartLevelNumber;
    
    private DS_PlayerPersistent _playerPersistent;

    [Button]
    public void UpdateLevelNodes(int currentLevelIndex)
    {
        _playerPersistent = GlobalData.GetData<DS_PlayerPersistent>();
        
        for (int i = 0; i < Nodes.Length; i++)
        {
            bool locked = currentLevelIndex + 1 < StartLevelNumber + i;
            Nodes[i].SetLocked(locked);
            Nodes[i].SetLevelNumber(StartLevelNumber + i);

            if (!locked)
            {
                int levelIndex = StartLevelNumber + i - 1;
                float levelCompleteHealthPercentage = _playerPersistent.LevelCompletionProgress.ContainsKey(levelIndex)
                    ? _playerPersistent.LevelCompletionProgress[levelIndex]
                    : 0;
                
                if(levelCompleteHealthPercentage >= 1)
                {
                    Nodes[i].SetStars(3);
                }
                else if(levelCompleteHealthPercentage >= 0.5f)
                {
                    Nodes[i].SetStars(2);
                }
                else if(levelCompleteHealthPercentage > 0)
                {
                    Nodes[i].SetStars(1);
                }
                else
                {
                    Nodes[i].SetStars(0);
                }
            }
            
        }
    }
}
