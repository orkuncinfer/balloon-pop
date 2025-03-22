using UnityEngine;

public class InGameBuff : MonoCore
{
    [SerializeField] private EventField<string> _onBuffSelected;
    [SerializeField] private ItemDefinition _buffItem;
    
    protected bool _isBuffActive;
    

    protected override void OnGameModeStarted()
    {
        base.OnGameModeStarted();
        _isBuffActive = false;
        if(BuffValidator.HasBuff(_buffItem))
            _isBuffActive = true;
        Debug.Log("listening buff select event");
        _onBuffSelected.Register(null,OnSelected);

        DefaultPlayerInventory.Instance.onItemAdded += OnItemAdded;
    }

    private void OnItemAdded(string arg1, int arg2, int arg3)
    {
        OnSelected(default,arg1);
    }

    protected override void OnGameModeStopped()
    {
        base.OnGameModeStopped();
        _isBuffActive = false;
        DefaultPlayerInventory.Instance.onItemAdded -= OnItemAdded;
        _onBuffSelected.Unregister(null,OnSelected);
    }
    
    private void OnSelected(EventArgs arg1, string arg2)
    {
        Debug.Log("Buff selected "  + arg2);
        if (BuffValidator.HasBuff(_buffItem))
        {
            _isBuffActive = true;
            OnBuffSelected();
        }
    }

    protected virtual void OnBuffSelected()
    {
        
    }
}