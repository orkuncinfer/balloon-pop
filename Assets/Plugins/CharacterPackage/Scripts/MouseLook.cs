using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    
    [Header("Rotation Clamping")]
    public float minXRotation = -45f; // Lower limit for X-axis rotation
    public float maxXRotation = 45f;  // Upper limit for X-axis rotation
    
    private float xRotation = 0f;     // Current X-axis rotation

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Apply Y-axis rotation (looking left-right)
        transform.Rotate(Vector3.up * mouseX);

        // Apply X-axis rotation (looking up-down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);  // Clamp X-axis rotation

        // Apply clamped X-axis rotation to the transform
        transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y, 0f);
    }
}