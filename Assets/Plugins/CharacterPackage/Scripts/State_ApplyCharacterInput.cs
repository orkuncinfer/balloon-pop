using System.Collections;
using System.Collections.Generic;
using ECM2;
using UnityEngine;
using UnityEngine.InputSystem;

public class State_ApplyCharacterInput : MonoState
{
    public Character Character;
        public InputActionAsset ActionAsset;
        public bool BlockInput;
        protected InputAction movementInputAction { get; set; }
        public Vector2 MoveInput;
        [SerializeField] private bool _isCameraRelative = true;
        private Camera _mainCam;
        [SerializeField] private bool _orientToCamera = true;
        
        [SerializeField] private float _orientationSpeed = 10f;

        protected override void OnEnter()
        {
            base.OnEnter();
            SetupPlayerInput();
            _mainCam = Camera.main;
            movementInputAction?.Enable();
            
            StaticUpdater.onUpdate += OnStaticUpdate;
        }

        protected override void OnExit()
        {
            base.OnExit();
            movementInputAction?.Disable();
            Character.SetMovementDirection(Vector3.zero);
            StaticUpdater.onUpdate -= OnStaticUpdate;
        }
        
        private void OnStaticUpdate()
        {
            Vector2 inputMove = GetMovementInput();
            MoveInput = inputMove;
            Vector3 movementDirection = Vector3.zero;
            if (_isCameraRelative)
            {
                Vector3 cameraForward = _mainCam.transform.forward;
                Vector3 cameraRight = _mainCam.transform.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();

                // Calculate movement direction relative to the camera's orientation
                movementDirection = cameraForward * inputMove.y + cameraRight * inputMove.x;
            }
            else
            {
                movementDirection =  Vector3.zero;
                movementDirection += Vector3.right * inputMove.x;
                movementDirection += Vector3.forward * inputMove.y;
            }

            if (_orientToCamera)
            {
                Character.RotateTowards(_mainCam.transform.forward, Time.deltaTime * _orientationSpeed);
            }
            
            // Obtain the camera's forward and right vectors, but ignore the y component to keep movement horizontal.
            

            Character.SetMovementDirection(movementDirection);
            
          /*  if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
                Character.Crouch();
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
                Character.UnCrouch();
            
            if (Input.GetButtonDown("Jump"))
                Character.Jump();
            else if (Input.GetButtonUp("Jump"))
                Character.StopJumping();*/
        }
        
        protected virtual void SetupPlayerInput()
        {
            if (ActionAsset == null)
                return;
            movementInputAction = ActionAsset.FindAction("Movement");
        }
        private void OnPerformed(InputAction.CallbackContext obj)
        {
        }
        public virtual Vector2 GetMovementInput()
        {
            if (BlockInput) return Vector2.zero;

            if (movementInputAction != null && ActionAsset != null)
            {
                return movementInputAction.ReadValue<Vector2>().normalized;
            }
            return MoveInput;
        }
}
