using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractionObjectEquipment : MonoBehaviour
{
    [SerializeField] private  InteractionObject _interactionObject;
    private Equipable _equipable;

    public void PickUp()
    {
        ActorBase actor =
            ActorUtilities.FindFirstActorInParents(_interactionObject.lastUsedInteractionSystem.transform);
        _equipable.EquipThisInstance(actor);
    }
}
