using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    // Reference to the Rigidbody component
    private Rigidbody _rigidbody;

    // Reference to the camera, you can assign it through the inspector or find it in Start
    public Transform CameraTransform;

    private void Start()
    {
        // Get the Rigidbody component
        _rigidbody = GetComponent<Rigidbody>();

        // Optionally, find the main camera if not assigned in the inspector
        if (CameraTransform == null)
        {
            CameraTransform = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        RotateTowardsCamera();
    }

    private void RotateTowardsCamera()
    {
        // Get the forward direction of the camera
        Vector3 cameraForward = CameraTransform.forward;

        // Zero out the Y component to only rotate on the Y axis
        cameraForward.y = 0;

        // Normalize the direction vector to prevent scaling issues
        cameraForward.Normalize();

        // Create a rotation looking in the direction of the camera's forward vector
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        // Apply the rotation to the character's Rigidbody
        _rigidbody.MoveRotation(targetRotation);
    }
}