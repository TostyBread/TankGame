using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementAndBehaviour : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints to patrol
    public float moveSpeed = 2f;  // Speed of movement
    public float rotationSpeed = 50f; // Hull rotation speed
    public float turretRotationSpeed = 30f; // Turret rotation speed
    public float waypointTolerance = 0.1f; // How close the tank needs to be to consider it has reached the waypoint

    public Transform player; // Reference to the player
    public float spottingRange = 5f; // Range within which the enemy spots the player
    public bool hasTurret = false; // Whether the tank has a turret
    public Transform turret; // Reference to the turret if the tank has one

    public Transform raycastOrigin; // Transform from which the raycast will be cast
    public float raycastDistance = 1f; // Distance of the raycast
    public LayerMask obstacleLayer; // Layer of obstacles (e.g., other tanks)

    private int currentWaypointIndex = 0; // Index of the current waypoint
    private bool isRotating = true; // Whether the tank is currently rotating
    private bool playerInRange = false; // Whether the player is in range
    private bool reorienting = false; // Whether the tank is reorienting to the waypoint
    private bool isBlocked = false; // Whether the tank is blocked by another tank

    void FixedUpdate()
    {
        CheckForObstacles(); // Check for obstacles using raycast
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        CheckForPlayer(); // Check if player is in range

        if (isBlocked)
        {
            // Stop moving if blocked
            if (isRotating) RotateTowardsWaypoint();

            if (playerInRange) // Still lets enemy target despite being blocked
            {
                if (hasTurret) // if enemy tank has turret
                {
                    RotateTurretTowardsPlayer();
                }
                else
                {
                    RotateTowardsPlayer();
                }
            }
        }
        else
        {
            if (playerInRange)
            {
                if (hasTurret) // if enemy tank has turret
                {
                    RotateTurretTowardsPlayer();
                    if (!reorienting) // Only move if not reorienting
                    {

                        RotateTowardsWaypoint();
                        MoveTowardsWaypoint();
                    }
                }
                else
                {
                    RotateTowardsPlayer();
                }
            }
            else
            {
                if (reorienting) // if rotate back to waypoint after rotate towards player
                {
                    RotateTowardsWaypoint();
                }
                else if (isRotating) // if is still rotating
                {
                    RotateTowardsWaypoint();
                }
                else // otherwise move to the next waypoint
                {
                    MoveTowardsWaypoint();
                }
            }
        }
    }

    void CheckForPlayer()
    {
        if (Vector2.Distance(transform.position, player.position) < spottingRange)
        {
            playerInRange = true;
            reorienting = false; // Stop reorienting if the player is back in range
        }
        else
        {
            if (playerInRange) // If the player just left the range
            {
                playerInRange = false;
                reorienting = true; // Start reorienting to the waypoint
            }
        }
    }

    void CheckForObstacles()
    {
        if (raycastOrigin == null) return; // Ensure the raycast origin is set

        // Raycast from the specified origin
        Vector2 direction = raycastOrigin.up; // Front direction of the raycast origin
        RaycastHit2D hit = Physics2D.CircleCast(raycastOrigin.position, 0.5f, transform.forward, raycastDistance, obstacleLayer);

        if (hit.collider != null)
        {
            isBlocked = true; // Stop movement if an obstacle is detected
        }
        else
        {
            isBlocked = false; // Resume movement if no obstacle is detected
        }
    }

    void RotateTowardsPlayer()
    {
        // closing up hull rotation towards player
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the 90 degree offset to align the tank's forward direction
        angle -= 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RotateTurretTowardsPlayer()
    {
        if (turret == null) return;

        // closing up turret distance to player position
        Vector2 direction = (player.position - turret.position).normalized; 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the 180 degree offset to align the turret's forward direction
        angle -= 180f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRotation, turretRotationSpeed * Time.deltaTime);
    }

    void RotateTowardsWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex]; // takes the transform coordinates from current index in array
        Vector2 direction = (targetWaypoint.position - transform.position).normalized; // rotate tank hull towards waypoint
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the 90 degree offset to align the tank's forward direction
        angle -= 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isRotating = false;
            reorienting = false; // Reorientation is complete
        }
    }

    void MoveTowardsWaypoint()
    {
        if (isRotating) return; // Do not move if still rotating

        Transform targetWaypoint = waypoints[currentWaypointIndex]; // takes the transform coordinates from current index in array
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime); 

        if (Vector2.Distance(transform.position, targetWaypoint.position) < waypointTolerance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // move to the next index in array of waypoints
            isRotating = true;
        }
    }
}