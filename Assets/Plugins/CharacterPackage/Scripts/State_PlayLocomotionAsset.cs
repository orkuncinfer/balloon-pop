using System.Collections;
using System.Collections.Generic;
using Animancer;
using ECM2;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class State_PlayLocomotionAsset : MonoState
{
    [SerializeField] private DSGetter<Data_RefVar> _locomotionAsset;
    [SerializeField] private  float _interpolationSpeed = 5f;
    [SerializeField][ReadOnly] private Vector3 _velocityDebug;
    public InputActionAsset ActionAsset;
    private Character _character;
    protected InputAction movementInputAction { get; set; }
    public Vector2 MoveInput;
    private Data_Animancer _dataAnimancer;
    private LinearMixerTransitionAsset _asset;
    private MixerTransition2DAsset _asset2D;
    private bool _idling;
    private Camera _mainCam;

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
       
        PlayLocomotionAsset();
        
        _mainCam = Camera.main;
        _locomotionAsset.Data.onValueChanged += OnLocomotionAssetChanged;
    }

    private void OnLocomotionAssetChanged(Object arg1, Object arg2)
    {
        PlayLocomotionAsset();
    }

    void PlayLocomotionAsset()
    {
        if (_locomotionAsset.Data.Value is LinearMixerTransitionAsset linear)
        {
            _asset = linear;
            var state =_dataAnimancer.AnimancerComponent.Play(_asset);
            state.ApplyFootIK = true;
        }else if (_locomotionAsset.Data.Value is MixerTransition2DAsset mixer)
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
        
        
        
        if (_dataAnimancer.AnimancerComponent.States.Current is LinearMixerState linearMixerState)
        {
            float velocity = _character.GetVelocity().magnitude;
            linearMixerState.Parameter = Mathf.Lerp(linearMixerState.Parameter, velocity, Time.deltaTime * _interpolationSpeed);
        }
        if(_asset2D != null)
        {
            if (_dataAnimancer.AnimancerComponent.States.Current is CartesianMixerState mixerState2D)
            {
                Vector2 parameter = new Vector2(_velocityDebug.x, _velocityDebug.z);
                mixerState2D.Parameter = Vector3.Lerp(mixerState2D.Parameter, parameter, Time.deltaTime * _interpolationSpeed);
            }
        }
    }
    
    Vector3 GetWorldSpaceMovement(Vector3 relativeMoveDirection)
    {
        // Get the camera's forward and right vectors
        Vector3 cameraForward = _mainCam.transform.forward;
        Vector3 cameraRight = _mainCam.transform.right;

        // Ignore the y-axis to keep movement on the horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;
    
        // Normalize the vectors to ensure proper scaling
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Create a rotation matrix that represents the camera's current orientation relative to the world
        Matrix4x4 cameraRotationMatrix = new Matrix4x4(
            new Vector4(cameraRight.x, cameraRight.y, cameraRight.z, 0),
            new Vector4(0, 1, 0, 0), // Y-axis (up) stays the same
            new Vector4(cameraForward.x, cameraForward.y, cameraForward.z, 0),
            new Vector4(0, 0, 0, 1)
        );

        // Use the inverse of the camera's rotation matrix to transform the relative movement direction
        Matrix4x4 inverseCameraMatrix = cameraRotationMatrix.inverse;

        // Convert the camera-relative movement direction to a world-space direction
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
