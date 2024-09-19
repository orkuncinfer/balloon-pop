using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class InteractionObjectEquipment : MonoBehaviour
{
    [SerializeField] private  InteractionObject _interactionObject;
    [SerializeField] private Weapon _weapon;

    public void PickUp()
    {
        ActorBase actor =
            ActorUtilities.FindFirstActorInParents(_interactionObject.lastUsedInteractionSystem.transform);
        _weapon.EquipThisInstance(actor);
    }
}
