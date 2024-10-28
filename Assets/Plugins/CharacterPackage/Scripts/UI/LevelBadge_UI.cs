using TMPro;
using UnityEngine;

public class LevelBadge_UI : MonoState
{
    [SerializeField] private TextMeshProUGUI _levelText;
    private Data_GAS _characterData;
    protected override void OnEnter()
    {
        base.OnEnter();
        _characterData = Owner.GetData<Data_GAS>();
        OnLevelChanged();
        _characterData.LevelController.levelChanged += OnLevelChanged;
    }

    private void OnLevelChanged()
    {
        _levelText.text = _characterData.LevelController.level.ToString();
    }

    protected override void OnExit()
    {
        base.OnExit();
        _characterData.LevelController.levelChanged -= OnLevelChanged;
    }
}