using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolMovement : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints to patrol
    public float moveSpeed = 2f;  // Speed of movement
    public float rotationSpeed = 50f; // Rotation speed
    public float waypointTolerance = 0.1f; // How close the tank needs to be to consider it has reached the waypoint
    public Transform player; // Reference to the player
    public float spottingRange = 5f; // Range within which the enemy spots the player

    private int currentWaypointIndex = 0; // Index of the current waypoint
    private bool isRotating = true; // Whether the tank is currently rotating
    private bool playerInRange = false; // Whether the player is in range
    private bool reorienting = false; // Whether the tank is reorienting to the waypoint

    void Update()
    {
        if (waypoints.Length == 0) return;

        CheckForPlayer();

        if (playerInRange)
        {
            RotateTowardsPlayer();
        }
        else
        {
            if (reorienting)
            {
                RotateTowardsWaypoint();
            }
            else if (isRotating)
            {
                RotateTowardsWaypoint();
            }
            else
            {
                MoveTowardsWaypoint();
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

    void RotateTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the 90 degree offset to align the tank's forward direction
        angle -= 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RotateTowardsWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
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
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < waypointTolerance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            isRotating = true;
        }
    }
}


