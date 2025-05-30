using ECM2;
using UnityEngine;

public class AbilityAction_IsCancelable : AbilityAction
{
    [SerializeField] private GameplayTagContainer _tagsToAdd;
    
    public override AbilityAction Clone()
    {
        AbilityAction_IsCancelable clone = AbilityActionPool<AbilityAction_IsCancelable>.Shared.Get();
        clone.EventName = EventName;
        clone._tagsToAdd = _tagsToAdd;
        return clone;
    }

    public override void OnStart(Actor owner, ActiveAbility ability)
    {
        base.OnStart(owner, ability);
        ActiveAbility.CanBeCanceled = true;
    }
    
    public override void OnTick(Actor owner)
    {
        base.OnTick(owner); 
    }

    public override void OnExit()
    {
        base.OnExit();
        ActiveAbility.CanBeCanceled = false;
    }
}