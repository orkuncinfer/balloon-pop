using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class State_SetCurrentLevelText : MonoState
{
    public TextMeshProUGUI[] LevelTexts;

    protected override void OnEnter()
    {
        base.OnEnter();
        foreach (var text in LevelTexts)
        {
            text.text = $"LEVEL {GlobalData.GetData<DS_PlayerPersistent>().CurrentLevelIndex + 1}";
        }
    }
}
