using UnityEngine;

public class AimingReticle : MonoBehaviour
{
    public Transform tankTurret; // Reference to the tank's turret
    public Camera mainCamera; // Reference to the main camera
    public LayerMask obstacleMask; // Layer mask for obstacles
    public float aimOffset = 0.1f; // Offset to prevent reticle from sticking to obstacles

    private void Update()
    {
        UpdateReticlePosition();
    }

    private void UpdateReticlePosition()
    {
        // Get the mouse position in world space
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure z is zero for 2D

        // Convert to Vector2 for 2D calculations
        Vector2 mouseWorldPosition2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        // Calculate the direction from the turret to the mouse
        Vector2 directionToMouse = mouseWorldPosition2D - (Vector2)tankTurret.position;

        // Check if the turret is facing towards the mouse
        float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        float turretAngle = tankTurret.eulerAngles.z;

        // Determine if the turret is aligned close to the mouse position
        bool isCloseToMouse = Mathf.Abs(Mathf.DeltaAngle(turretAngle, angleToMouse)) < 10f;

        if (isCloseToMouse)
        {
            // Perform a raycast to check for obstacles
            RaycastHit2D hit = Physics2D.Raycast((Vector2)tankTurret.position, directionToMouse.normalized, directionToMouse.magnitude, obstacleMask);

            if (hit.collider != null)
            {
                // Move reticle to the point where the raycast hit, but with an offset to avoid sticking to the obstacle
                transform.position = (Vector2)hit.point - directionToMouse.normalized * aimOffset;
            }
            else
            {
                // No obstacle hit, move reticle to mouse position
                transform.position = mouseWorldPosition2D;
            }
        }
        else
        {
            // If not close to alignment, position reticle where the turret is aiming
            // You might want to adjust the distance based on your game's requirements
            transform.position = (Vector2)tankTurret.position + (Vector2)tankTurret.up * 5f; // Example distance
        }
    }
}
