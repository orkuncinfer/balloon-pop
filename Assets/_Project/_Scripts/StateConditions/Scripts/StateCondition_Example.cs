using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SOCreatable("Conditions")]
public class StateCondition_Example : ConditionDefinition
{
    public override bool IsConditionMet(ActorBase actor)
    {
        return true;
    }
}
