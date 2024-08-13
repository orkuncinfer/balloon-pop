using StatSystem;

public class ItemAction_ModifyAttackSpeed : SimpleAction
{
    public float ModifierValue;
    public ModifierOperationType ModifierOperation;

    public override void OnAction(ActorBase owner)
    {
        base.OnAction(owner);
        DS_PlayerPersistent playerData = GlobalData.GetData<DS_PlayerPersistent>();
        DS_PlayerRuntime playerRuntime = owner.GetData<DS_PlayerRuntime>();
        playerRuntime.AttackSpeedRuntime += ModifierValue;
        playerData.SaveData();
    }


}