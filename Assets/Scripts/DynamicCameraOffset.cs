using UnityEngine;
using Cinemachine;

public class DynamicCameraOffsetController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float offsetMultiplier = 0.5f; // Adjust this value to control the effect intensity

    private CinemachineCameraOffset cameraOffsetComponent;

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

        // Convert the mouse position to world space
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure the z-coordinate is set to 0 for a 2D game

        // Get the current camera position and tank position
        Vector3 cameraPosition = virtualCamera.transform.position;
        Vector3 tankPosition = virtualCamera.Follow.position;

        // Calculate the direction from the tank to the mouse pointer
        Vector3 directionToMouse = mouseWorldPosition - tankPosition;

        // Apply an offset based on the direction to the mouse pointer
        Vector3 dynamicOffset = directionToMouse * offsetMultiplier;

        // Update the camera offset component
        cameraOffsetComponent.m_Offset = dynamicOffset;
    }
}