using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class AimIKWeightHandler : MonoBehaviour
{
    public bool IsAiming;
    public float LerpSpeed = 10f;
    private AimIK _aimIK;

    public FullBodyBipedIK ik;

    private Camera _camera;
    
    [Range(0f, 1f)] public float headLookWeight = 1f;
    [Range(0f, 1f)] public float gunHoldWeight = 1f;
    
    public Vector3 gunHoldOffset;
    public Vector3 leftHandOffset;
    public float _leftHandBendSpeed;
    public Recoil recoil;
    public bool HoldLeftHand = true;
    private Vector3 headLookAxis;
    private Vector3 leftHandPosRelToRightHand;
    private Quaternion leftHandRotRelToRightHand;
    private Vector3 aimTarget;
    private Quaternion rightHandRotation;

    public Transform LeftHandBendTarget;
    
    private float _aimIKWeight;
    private float _leftHandBendWeight;
    private void Start()
    {
        _aimIK = GetComponent<AimIK>();
        
        _camera = Camera.main;
    }

    private void Update()
    {
        //lerp weight
        _aimIKWeight = Mathf.MoveTowards(_aimIKWeight, IsAiming ? 1 : 0, Time.deltaTime * LerpSpeed);
        _leftHandBendWeight = Mathf.MoveTowards(_leftHandBendWeight, IsAiming ? 1 : 0, Time.deltaTime * _leftHandBendSpeed);
        
        _aimIK.solver.IKPositionWeight = _aimIKWeight;

        if (HoldLeftHand)
        {
	        if (ik.solver.leftHandEffector.target != null)
	        {
		        //ik.solver.leftHandEffector.positionWeight = 1;
	        }
        }
        if (LeftHandBendTarget)
        {
	        ik.solver.leftArmChain.bendConstraint.bendGoal = LeftHandBendTarget;
	        ik.solver.leftArmChain.bendConstraint.weight = _leftHandBendWeight;
	        ik.solver.leftHandEffector.rotationWeight = _aimIKWeight;
        }
    }

    private void LateUpdate()
    {
	    if (_aimIKWeight > 0)
	    {
		    
		    Vector3 lookDirection = _camera.transform.forward;
		    aimTarget = _camera.transform.position + (lookDirection * 10f);
		   // 
		    Read();

		    // AimIK pass
		    AimIK();

		    // FBBIK pass - put the left hand back to where it was relative to the right hand before AimIK solved
		    FBBIK();

		    // AimIK pass
		    AimIK();

		    // Rotate the head to look at the aim target
		    HeadLookAt(aimTarget);
	    }
    }

    public void ToggleAiming(bool isAiming,bool isInstant = false, bool releaseLeftHand = false)
    {
	    Debug.Log("toggled aiming" + isAiming);
        IsAiming = isAiming;

        if (isAiming && isInstant)
        {
	        _aimIKWeight = 1;
	        _leftHandBendWeight = 1;
	        _aimIK.solver.IKPositionWeight = 1;
        }
        else if(!isAiming && isInstant)
        {
	        _aimIKWeight = 0;
	        _leftHandBendWeight = 0;
	        _aimIK.solver.IKPositionWeight = 0;
        }
        
        if (isAiming)
        {
	        //_aimIK.enabled = false;
	        //ik.enabled = false;
            ik.solver.OnPreRead += OnPreRead;
        }
        else
        {
            ik.solver.OnPreRead -= OnPreRead;
        }

        if (releaseLeftHand)
        {
	        ik.solver.leftHandEffector.positionWeight = 0;
        }
    }

    private void Read() {
			// Remember the position and rotation of the left hand relative to the right hand
			leftHandPosRelToRightHand = ik.references.rightHand.InverseTransformPoint(ik.references.leftHand.position);
			leftHandRotRelToRightHand = Quaternion.Inverse(ik.references.rightHand.rotation) * ik.references.leftHand.rotation;
		}

		private void AimIK() {
			// Set AimIK target position and update
			_aimIK.solver.IKPosition = aimTarget;
			_aimIK.solver.Update(); // Update AimIK
		}

		// Positioning the left hand on the gun after aiming has finished
		private void FBBIK() {
			// Store the current rotation of the right hand
			rightHandRotation = ik.references.rightHand.rotation;

			// Offsetting hands, you might need that to support multiple weapons with the same aiming pose
			Vector3 rightHandOffset = ik.references.rightHand.rotation * gunHoldOffset * gunHoldWeight * _aimIKWeight;
			ik.solver.rightHandEffector.positionOffset += rightHandOffset;

			if (recoil != null) recoil.SetHandRotations(rightHandRotation * leftHandRotRelToRightHand, rightHandRotation);

			// Update FBBIK
			ik.solver.Update();
			
			// Rotating the hand bones after IK has finished
			if (recoil != null) {
				ik.references.rightHand.rotation = recoil.rotationOffset * rightHandRotation;
				ik.references.leftHand.rotation = recoil.rotationOffset * rightHandRotation * leftHandRotRelToRightHand;
			} else {
				ik.references.rightHand.rotation = rightHandRotation;
				ik.references.leftHand.rotation = rightHandRotation * leftHandRotRelToRightHand;
			}
		}

		// Final calculations before FBBIK solves. Recoil has already solved by, so we can use its calculated offsets. 
		// Here we set the left hand position relative to the position and rotation of the right hand.
		private void OnPreRead() {
			Quaternion r = recoil != null? recoil.rotationOffset * rightHandRotation: rightHandRotation;
			Vector3 leftHandTarget = ik.references.rightHand.position + ik.solver.rightHandEffector.positionOffset + r * leftHandPosRelToRightHand;
			ik.solver.leftHandEffector.positionOffset += leftHandTarget - ik.references.leftHand.position - ik.solver.leftHandEffector.positionOffset + r * leftHandOffset * _aimIKWeight;
		}

		// Rotating the head to look at the target
		private void HeadLookAt(Vector3 lookAtTarget) {
			Quaternion headRotationTarget = Quaternion.FromToRotation(ik.references.head.rotation * headLookAxis, lookAtTarget - ik.references.head.position);
			ik.references.head.rotation = Quaternion.Lerp(Quaternion.identity, headRotationTarget, headLookWeight * _aimIKWeight) * ik.references.head.rotation;
		}

}