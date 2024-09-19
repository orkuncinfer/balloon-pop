using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimatorTest : MonoBehaviour
{
     private Animator _animator;

     // Animator Override Controller for swapping animations
     private AnimatorOverrideController _overrideController;

     // The new animation clip to assign
     [SerializeField] private AnimationClip newAbilityClip;

     // The name of the state where we want to replace the animation
     private const string AbilityStateName = "Ability";

     private void Awake()
     {
          // Get the Animator component attached to this GameObject
          _animator = GetComponent<Animator>();

          // Create an AnimatorOverrideController based on the current controller
          _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

          // Assign the override controller back to the animator
          _animator.runtimeAnimatorController = _overrideController;
     }
     [Button]
     public void ChangeAbilityClip()
     {
          // Get the list of all animations in the override controller
          var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

          // Populate the list with the current animations
          _overrideController.GetOverrides(overrides);

          // Find and replace the animation in the "Ability" state
          for (int i = 0; i < overrides.Count; i++)
          {
               if (overrides[i].Key.name == AbilityStateName)
               {
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, newAbilityClip);
                    break;
               }
          }

          // Apply the changes
          _overrideController.ApplyOverrides(overrides);
          _animator.SetTrigger("UseAbility");
     }
}
