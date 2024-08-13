using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class State_UI_LevelProgressBind : MonoState
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private BP_PlayerLevelableSO _levelData;

    protected override void OnEnter()
    {
        base.OnEnter();
        _levelData.onExpChanged += OnExpChanged;
        UpdateProgress();
    }

    protected override void OnExit()
    {
        base.OnExit();
        _levelData.onExpChanged -= OnExpChanged;
    }
    
    private void OnExpChanged(float obj)
    {
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        float progress = _levelData.CurrentExperience / _levelData.GetExperienceForLevel(_levelData.Level + 1);
        _fillImage.fillAmount = progress;
        if(_levelText) _levelText.text = $"{_levelData.Level}";
    }
}
