using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_HandleShieldCellVisual : MonoState
{
    [SerializeField] private GameObject _shieldVisual;
    [SerializeField] private GenericKey _hasShieldTag;
    protected override void OnEnter()
    {
        base.OnEnter();
        _shieldVisual.SetActive(true);
        Owner.onTagsChanged += OnTagsChanged;
    }

    protected override void OnExit()
    {
        base.OnExit();
        Owner.onTagsChanged -= OnTagsChanged;
    }

    private void OnTagsChanged(ITagContainer arg1, string arg2)
    {
        if (!Owner.ContainsTag(_hasShieldTag.ID))
        {
            _shieldVisual.SetActive(false);
        }
    }
}
