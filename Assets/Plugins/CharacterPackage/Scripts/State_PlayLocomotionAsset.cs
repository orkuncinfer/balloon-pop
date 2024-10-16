using System.Collections;
using System.Collections.Generic;
using Animancer;
using ECM2;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class State_PlayLocomotionAsset : MonoState
{
    private enum AnimationState
    {
        Idle,
        Locomotion
    }

    [SerializeField] private DSGetter<Data_RefVar> _locomotionAsset;
    [SerializeField] private float _interpolationSpeed = 5f;
    [SerializeField] [ReadOnly] private Vector3 _velocityDebug;
    [SerializeField] private float _velocityThreshold = 2f;
    public InputActionAsset ActionAsset;
    private Character _character;
    protected InputAction movementInputAction { get; set; }
    public Vector2 MoveInput;
    private Data_Animancer _dataAnimancer;
    private LocomotionAsset _locomotionAssetData;
    private LinearMixerTransitionAsset _asset;
    private MixerTransition2DAsset _asset2D;
    private Camera _mainCam;

    // State management variables
    private AnimationState _currentState;
    private AnimationState _previousState;


    protected override void OnEnter()
    {
        base.OnEnter();
        _character = Owner.GetComponent<Character>();
        _dataAnimancer = Owner.GetData<Data_Animancer>();
        _locomotionAsset.GetData(Owner);
        _dataAnimancer.AnimancerComponent.Layers[0].ApplyFootIK = true;
        _dataAnimancer.AnimancerComponent.Layers[0].ApplyAnimatorIK = true;
        movementInputAction = ActionAsset.FindAction("Movement");
        movementInputAction?.Enable();

        _mainCam = Camera.main;
        _locomotionAssetData = _locomotionAsset.Data.Value as LocomotionAsset;
        _locomotionAsset.Data.onValueChanged += OnLocomotionAssetChanged;
        // Start with Idle state
        PlayIdle();
        ChangeState(AnimationState.Idle);
    }

    private void OnLocomotionAssetChanged(Object arg1, Object arg2)
    {
        _locomotionAssetData = _locomotionAsset.Data.Value as LocomotionAsset;
        OnEnterState(_currentState);
    }

    void PlayLocomotionAsset()
    {
        if (_locomotionAssetData.Locomotion is LinearMixerTransitionAsset linear)
        {
            _asset = linear;
            var state = _dataAnimancer.AnimancerComponent.Play(_asset);
            state.ApplyFootIK = true;
        }
        else if (_locomotionAssetData.Locomotion is MixerTransition2DAsset mixer)
        {
            _asset2D = mixer;
            var state = _dataAnimancer.AnimancerComponent.Play(_asset2D);
            state.ApplyFootIK = true;
        }
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        _velocityDebug = GetWorldSpaceMovement(_character.GetVelocity());
        Vector2 inputMove = GetMovementInput();
        MoveInput = inputMove.normalized;

        float velocity = _character.GetVelocity().magnitude;
        
        if (velocity < _velocityThreshold)
        {
            ChangeState(AnimationState.Idle);
        }
        else
        {
            ChangeState(AnimationState.Locomotion);
        }
        // Locomotion parameter update when in locomotion state
        if (_currentState == AnimationState.Locomotion)
        {
            if (_dataAnimancer.AnimancerComponent.States.Current is LinearMixerState linearMixerState)
            {
                linearMixerState.Parameter = Mathf.Lerp(linearMixerState.Parameter, velocity,
                    Time.deltaTime * _interpolationSpeed);
            }

            if (_asset2D != null &&
                _dataAnimancer.AnimancerComponent.States.Current is CartesianMixerState mixerState2D)
            {
                Vector2 parameter = new Vector2(_velocityDebug.x, _velocityDebug.z);
                mixerState2D.Parameter =
                    Vector3.Lerp(mixerState2D.Parameter, parameter, Time.deltaTime * _interpolationSpeed);
            }
        }
    }

    // Method to handle state transitions
    private void ChangeState(AnimationState newState)
    {
        if (_currentState == newState) return;

        OnExitState(_currentState); // Execute exit logic for the current state
        _previousState = _currentState;
        _currentState = newState;
        OnEnterState(_currentState); // Execute enter logic for the new state
    }

    // Called when entering a new state
    private void OnEnterState(AnimationState state)
    {
        switch (state)
        {
            case AnimationState.Idle:
                PlayIdle();
                break;
            case AnimationState.Locomotion:
                PlayLocomotionAsset();
                break;
        }
    }

    // Called when exiting the current state
    private void OnExitState(AnimationState state)
    {
        // Optionally, implement any logic you need when exiting a state
    }

    // Play the Idle animation
    private void PlayIdle()
    {
        var state = _dataAnimancer.AnimancerComponent.Play(_locomotionAssetData.Idle);
        state.Speed = 1f; // Set idle animation speed if needed
        state.ApplyFootIK = true;
    }

    Vector3 GetWorldSpaceMovement(Vector3 relativeMoveDirection)
    {
        Vector3 cameraForward = _mainCam.transform.forward;
        Vector3 cameraRight = _mainCam.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Matrix4x4 cameraRotationMatrix = new Matrix4x4(
            new Vector4(cameraRight.x, cameraRight.y, cameraRight.z, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(cameraForward.x, cameraForward.y, cameraForward.z, 0),
            new Vector4(0, 0, 0, 1)
        );

        Matrix4x4 inverseCameraMatrix = cameraRotationMatrix.inverse;

        Vector3 worldSpaceDirection = inverseCameraMatrix.MultiplyPoint3x4(relativeMoveDirection);

        return worldSpaceDirection;
    }

    public virtual Vector2 GetMovementInput()
    {
        if (movementInputAction != null && ActionAsset != null)
        {
            return movementInputAction.ReadValue<Vector2>().normalized;
        }

        return MoveInput;
    }
}