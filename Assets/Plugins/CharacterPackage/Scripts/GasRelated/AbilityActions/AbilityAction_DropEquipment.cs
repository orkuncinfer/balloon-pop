using UnityEngine;

public class AbilityAction_DropEquipment : AbilityAction
{
    public override AbilityAction Clone()
    {
        AbilityAction_DropEquipment clone = AbilityActionPool<AbilityAction_DropEquipment>.Shared.Get();
        return clone;
    }

    public override void Reset()
    {
        base.Reset();
    }

    public override void OnStart(Actor owner, ActiveAbility ability)
    {
        base.OnStart(owner, ability);
        owner.GetData<DS_EquipmentUser>().DropCurrent();
    }
}