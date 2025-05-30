using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

public class PickupTest : MonoBehaviour
{
    public Actor UserActor;
    public InteractionSystem InteractionSystem;
    public GameObject EquipmentInstance;
    public PlayableDirector PlayableDirector;
    public AbilityDefinition DisableAbility;
    
    [Button]
    public void PickedUp()
    {
        UserActor.GetData<DS_EquipmentUser>().LerpSpeed = .25f;
        UserActor.GetData<DS_EquipmentUser>().EquipmentPrefab = EquipmentInstance;
        InteractionSystem.OnInteractionStop += OnInteractionStop;
    }

    private void OnInteractionStop(FullBodyBipedEffector effectortype, InteractionObject interactionobject)
    {
        //PlayableDirector.Stop();
        UserActor.GetComponentInChildren<AbilityController>().AddAndTryActivateAbility(DisableAbility);
    }

    public void ResumeAll()
    {
        InteractionSystem.ResumeAll();
    }
}
