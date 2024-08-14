using UnityEngine;
using Cinemachine;

public class DynamicCameraOffsetController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float offsetMultiplier = 0.5f; // General offset multiplier
    public float yOffsetMultiplier = 1.0f; // Multiplier for y-axis offset
    public float smoothTime = 0.1f; // Time to smooth the offset

    private CinemachineCameraOffset cameraOffsetComponent;
    private Vector3 velocity = Vector3.zero; // Used by SmoothDamp

    private void Start()
    {
        // Get the CinemachineCameraOffset component from the virtual camera
        cameraOffsetComponent = virtualCamera.GetComponent<CinemachineCameraOffset>();
        if (cameraOffsetComponent == null)
        {
            Debug.LogError("CinemachineCameraOffset component not found on the virtual camera.");
        }
    }

    private void Update()
    {
        ApplyDynamicOffset();
    }

    private void ApplyDynamicOffset()
    {
        if (cameraOffsetComponent == null) return;

        // Check if Follow target is assigned
        if (virtualCamera.Follow == null) return;

        // Convert the mouse position to world space
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure the z-coordinate is set to 0 for a 2D game

        // Get the current camera position and tank position
        Vector3 tankPosition = virtualCamera.Follow.position;

        // Calculate the direction from the tank to the mouse pointer
        Vector3 directionToMouse = mouseWorldPosition - tankPosition;

        // Calculate the desired offset based on the direction to the mouse pointer
        Vector3 desiredOffset = new Vector3(
            directionToMouse.x * offsetMultiplier,
            directionToMouse.y * yOffsetMultiplier,
            0
        );

        // Smoothly interpolate between the current and desired offset
        cameraOffsetComponent.m_Offset = Vector3.SmoothDamp(
            cameraOffsetComponent.m_Offset,
            desiredOffset,
            ref velocity,
            smoothTime
        );
    }
}
