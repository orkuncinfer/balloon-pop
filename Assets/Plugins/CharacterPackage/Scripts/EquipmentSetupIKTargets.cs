using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Serialization;

public enum EEquipHand
{
    LeftHand,
    RightHand
}
public class EquipmentSetupIKTargets : MonoBehaviour
{
    [FormerlySerializedAs("_equipable")] [SerializeField] private Equippable _equippable;
    
    [SerializeField] private EEquipHand _equipHand;
    public Vector3 rightHandOffset;
    [SerializeField] private Transform _aimTransform;
    [SerializeField] private Transform _leftHandPivot;
    [SerializeField] private Transform _rightHandPivot;
    [SerializeField] private Transform _leftHandBendTarget;
    
    private FullBodyBipedIK ik;
    private AimIK _aimIK;
    private AimIKWeightHandler _aimIKWeightHandler;
    private Camera _camera;
    private void OnEnable()
    {
        _camera = Camera.main;
        _equippable.onEquipped += SetupIKTargets;
        _equippable.onUnequipped += OnUnequip;
    }


    private void OnDisable()
    {
        _equippable.onEquipped -= SetupIKTargets;
       
    }
    private void OnUnequip(ActorBase obj)
    {
        _aimIKWeightHandler.ToggleAiming(false);
        _equippable.onUnequipped -= OnUnequip;
    }

    public void SetupIKTargets(ActorBase actor)
    {
        ik = actor.GetComponentInChildren<FullBodyBipedIK>();
        _aimIKWeightHandler = actor.GetComponentInChildren<AimIKWeightHandler>();
        _aimIKWeightHandler.ToggleAiming(true);
        if (_aimTransform)
        {
            _aimIK = actor.GetComponentInChildren<AimIK>();
            _aimIK.solver.transform = _aimTransform;
        }

        if (_aimIKWeightHandler)
        {
            _aimIKWeightHandler.LeftHandBendTarget = _leftHandBendTarget;
        }
        
        if (_leftHandPivot)
        {
            
            ik.solver.leftHandEffector.target = _leftHandPivot;
        }
        
        if (_rightHandPivot)
        {
            ik.solver.rightHandEffector.target = _rightHandPivot;
        }

        if (ik.references.leftHand.TryGetComponent(out HandPoser handPoser))
        {
            handPoser.poseRoot = _leftHandPivot;
        }
        
        if (ik.references.rightHand.TryGetComponent(out HandPoser handPoserRight))
        {
            handPoserRight.poseRoot = _rightHandPivot;
        }
        
        switch (_equipHand)
        {
            case EEquipHand.LeftHand:
                ik.solver.rightHandEffector.positionWeight = 1;
                break;
            case EEquipHand.RightHand:
                ik.solver.leftHandEffector.positionWeight = 1;
                break;
        }
        
        //StaticUpdater.onLateUpdate += UpdateIK;
    }

    private void UpdateIK()
    {
        Vector3 lookDirection = _camera.transform.forward;
        Vector3 aimTarget = _camera.transform.position + (lookDirection * 10f);
        _aimIK.solver.IKPosition = aimTarget;
    }
}
