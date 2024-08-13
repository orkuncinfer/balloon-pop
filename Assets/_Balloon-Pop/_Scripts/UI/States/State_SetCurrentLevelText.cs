using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class State_SetCurrentLevelText : MonoState
{
    public TextMeshProUGUI[] LevelTexts;
    public string Prefix = "LEVEL ";
    public int LevelOffset = 1;

    protected override void OnEnter()
    {
        base.OnEnter();
        foreach (var text in LevelTexts)
        {
            int levelIndex = GlobalData.GetData<DS_GameModePersistent>().CurrentLevelIndex;
            text.text = Prefix + (levelIndex + LevelOffset).ToString();
        }
    }
}
