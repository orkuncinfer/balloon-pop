using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class FramingTranspozerCameraControls : MonoBehaviour
{
    public InputActionAsset ActionAsset;
    
    [Header("Rotation Settings")]
    public float RotationSpeed = 5f;
    public float MinVerticalAngle = -30f;
    public float MaxVerticalAngle = 60f; 
    
    [Header("Zoom Settings")]
    public float ZoomSpeed = 5f;
    public float MinZoomDistance = 3f;
    public float MaxZoomDistance = 20f; 

    private InputAction _rotateAction;
    private InputAction _zoomAction;
    
    private float _yaw;  
    private float _pitch;

    private CinemachineFramingTransposer _virtualCamera;

    private bool _lockedMouse;
    private Vector2 _storedCursorPos;

    private void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        transform.SetParent(null);
        
        _rotateAction = ActionAsset.FindAction("RotateCamera");
        _zoomAction = ActionAsset.FindAction("ZoomCamera");

        _zoomAction.performed += OnZoomPerformed;
        _rotateAction.performed += OnRotatePerformed;
        _rotateAction.Enable();
        _zoomAction.Enable();
        
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;
    }
    private void OnDestroy()
    {
        _rotateAction.performed -= OnRotatePerformed;
    }

    private void LateUpdate()
    {
        if (_lockedMouse)
        {
            Mouse.current.WarpCursorPosition(_storedCursorPos);
        }
    }

    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        if (!Mouse.current.rightButton.isPressed)
        {
            if (_lockedMouse)
            {
                _lockedMouse = false;
                Cursor.lockState = CursorLockMode.None;
                Mouse.current.WarpCursorPosition(_storedCursorPos);
            }
            return;
        }

        if (!_lockedMouse)
        {
            _lockedMouse = true;
            Cursor.lockState = CursorLockMode.Locked;
            _storedCursorPos = Mouse.current.position.value;
        }
        Vector2 delta = context.ReadValue<Vector2>();
        
        _yaw += delta.x * RotationSpeed * Time.deltaTime;
        _pitch -= delta.y * RotationSpeed * Time.deltaTime;
        
        _pitch = Mathf.Clamp(_pitch, MinVerticalAngle, MaxVerticalAngle);
        
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
    
    private void OnZoomPerformed(InputAction.CallbackContext inputValue)
    {
        float value = -inputValue.ReadValue<Vector2>().y;
        
        _virtualCamera.m_CameraDistance += value * Time.deltaTime * ZoomSpeed;
        
        _virtualCamera.m_CameraDistance = Mathf.Clamp(_virtualCamera.m_CameraDistance, MinZoomDistance, MaxZoomDistance);
    }
}