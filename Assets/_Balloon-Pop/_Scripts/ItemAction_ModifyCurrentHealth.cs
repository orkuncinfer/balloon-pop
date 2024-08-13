using StatSystem;

public class ItemAction_ModifyCurrentHealth : SimpleAction
{
    public int ModifierValue;

    public override void OnAction(ActorBase owner)
    {
        base.OnAction(owner);
        DS_PlayerRuntime playerRuntime = owner.GetData<DS_PlayerRuntime>();
        playerRuntime.CurrentHealth += ModifierValue;
    }
}