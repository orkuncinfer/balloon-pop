using ECM2;
using UnityEngine;

public class AbilityAction_AddGameplayTags : AbilityAction
{
    [SerializeField] private GameplayTagContainer _tagsToAdd;
    
    ActiveAbility _ability;

    private Character _character;
    public override AbilityAction Clone()
    {
        AbilityAction_AddGameplayTags clone = AbilityActionPool<AbilityAction_AddGameplayTags>.Shared.Get();
        clone.EventName = EventName;
        clone._character = _character;
        clone._tagsToAdd = _tagsToAdd;
        return clone;
    }

    public override void OnStart(Actor owner, ActiveAbility ability)
    {
        base.OnStart(owner, ability);
        owner.GameplayTags.AddTags(_tagsToAdd);
    }
    
    public override void OnTick(Actor owner)
    {
        base.OnTick(owner); 
    }

    public override void OnExit()
    {
        base.OnExit();
        if (Owner == null)
        {
            Debug.Log(Owner);
        }
        Owner.GameplayTags.RemoveTags(_tagsToAdd);
    }
}