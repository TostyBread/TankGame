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

    private Transform player;
    private const string playerTag = "Player"; //Fixed to find player tag
    public float spottingRange = 5f; // Range within which the enemy spots the player
    public bool hasTurret = false; // Whether the tank has a turret
    public Transform turret; // Reference to the turret if the tank has one

    public Transform raycastOrigin; // Transform from which the raycast will be cast
    public Transform raycastShootingPoint; // Enemy tank's firing based on their gun alignment
    public float raycastDistance = 1f; // Distance of the raycast
    public LayerMask obstacleLayer; // Layer of obstacles (e.g., other tanks)
    public LayerMask playerDetector; // Layer for detecting player

    public GameObject tankProjectile; // Tank projectile
    public float projectileSpeed = 50f; // Tank projectile speed
    public Transform firePos; // Projectile spawn location
    public float cooldownTime = 2f; // Cooldown time between shots
    private float cooldownTimer = 0f; // Timer to track cooldown
    public GameObject explosionPrefab; // Explosion prefab

    public AudioClip[] shootSFX; // Array of shooting sound effects

    private int currentWaypointIndex = 0; // Index of the current waypoint
    private bool isRotating = true; // Whether the tank is currently rotating
    private bool playerInRange = false; // Whether the player is in range
    private bool reorienting = false; // Whether the tank is reorienting to the waypoint
    private bool isBlocked = false; // Whether the tank is blocked by another tank

    public bool isDead = false; // Determine so that it restrict enemy movement

    private void Start()
    {
        // Initialize player reference
        UpdatePlayerTarget();
    }

    void FixedUpdate()
    {
        CheckForObstacles(); // Check for obstacles using raycast
    }

    void Update()
    {
        if (isDead) return; // If enemy health reaches 0, disable all the controls

        // Update player reference in case the active player changes
        UpdatePlayerTarget();

        if (player == null) return;

        CheckForPlayer(); // Check if player is in range

        // Update the cooldown timer
        cooldownTimer -= Time.deltaTime;

        // Calculate the direction with a 90-degree offset
        Vector3 direction = raycastShootingPoint.up; // Original direction
        Vector3 offsetDirection = new Vector3(-direction.y, direction.x, 0f); // 90-degree rotation to the left

        // Perform the raycast
        RaycastHit2D insight = Physics2D.Raycast(raycastShootingPoint.position, offsetDirection, 100f, playerDetector);

        // Calculate the 90-degree rotation offset for the projectile
        Quaternion rotationOffset = Quaternion.Euler(0, 0, 90f); // 90-degree rotation around the Z-axis
        Quaternion finalRotation = firePos.rotation * rotationOffset; // Apply the offset to the fire position's rotation

        if (insight.collider && playerInRange) // if player in range and raycast touches player collider, fire at player
        {
            if (cooldownTimer <= 0f) // check if cooldown is over, if yes, then shoot again
            {
                Instantiate(tankProjectile, firePos.position, finalRotation); // Use the adjusted rotation (enemy spawn projectile)
                PlayRandomSound(shootSFX);
                PlayExplosion();
                // Reapply cooldown again
                cooldownTimer = cooldownTime;
            }
        }

        if (isBlocked) // if enemy front raycast has been blocked by other enemy
        {
            // Stop moving if blocked
            if (isRotating && waypoints.Length != 0) RotateTowardsWaypoint();

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
            if (playerInRange) // if player is in spotting range
            {
                if (hasTurret) // if enemy tank has turret
                {
                    RotateTurretTowardsPlayer(); // rotate turret towards player
                    if (!reorienting && waypoints.Length != 0) // Only move if not reorienting and waypoints being available
                    {
                        RotateTowardsWaypoint();
                        MoveTowardsWaypoint();
                    }
                }
                else // if enemy doesn't have turret
                {
                    RotateTowardsPlayer();
                }
            }
            else
            {
                if (waypoints.Length == 0) // if there is no waypoint, simply just skip orentation 
                {
                    return;
                }
                else if (reorienting) // if rotate back to waypoint after rotate towards player
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

    // This code will find GameObject with Player tag on it and assign its transformation
    private void UpdatePlayerTarget() 
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    // Checks if player is in range
    void CheckForPlayer()
    {
        if (Vector2.Distance(transform.position, player.position) < spottingRange && player != null) // Uses Vector2.Distance to determine transform location
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

    // Handles Obstacle checking
    void CheckForObstacles()
    {
        if (raycastOrigin == null) return; // Ensure the raycast origin is set

        // Raycast from the specified origin
        Vector2 direction = raycastOrigin.up; // Front direction of the raycast origin
        RaycastHit2D hit = Physics2D.CircleCast(raycastOrigin.position, 0.5f, transform.forward, raycastDistance, obstacleLayer); // Raycast Circle check for obstacleLayer

        if (hit.collider != null) // if hit.collider is available
        {
            isBlocked = true; // Stop movement if an obstacle is detected
        }
        else
        {
            isBlocked = false; // Resume movement if no obstacle is detected
        }
    }

    private void PlayExplosion()
    {
        // Define the explosion rotation based on whether the object has a turret
        Quaternion explosionRotation;

        if (hasTurret && turret != null) // Check if the object has a turret
        {
            // Get the current rotation of the turret
            Quaternion turretRotation = turret.rotation;

            // Define a fixed rotation (e.g., no additional rotation)
            Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

            // Calculate the explosion rotation by combining the turret’s rotation with the fixed rotation
            explosionRotation = turretRotation * fixedRotation;
        }
        else // If the object does not have a turret
        {
            // Get the current rotation of the object's body
            Quaternion bodyRotation = transform.rotation;

            // Define a fixed rotation (e.g., adjust as needed for proper explosion orientation)
            Quaternion fixedRotation = Quaternion.Euler(0, 0, -90);

            // Calculate the explosion rotation by combining the body's rotation with the fixed rotation
            explosionRotation = bodyRotation * fixedRotation;
        }

        // Define the offset distance for the explosion position relative to the fire position
        Vector3 offset = new Vector3(-0.8f, 0, 0); // Adjust the offset as needed for the desired explosion effect

        // Calculate the final position of the explosion by applying the offset to the fire position
        Vector3 explosionPosition = firePos.position + firePos.right * offset.x + firePos.up * offset.y + firePos.forward * offset.z;

        // Start the coroutine to handle the explosion lifecycle with the calculated position and rotation
        StartCoroutine(HandleExplosion(explosionPosition, explosionRotation));
    }


    private IEnumerator HandleExplosion(Vector3 position, Quaternion rotation)
    {
        // Instantiate the explosion prefab at the given position and rotation
        GameObject explosion = Instantiate(explosionPrefab, position, rotation);

        // Get the Animator component from the instantiated explosion
        Animator explosionAnimator = explosion.GetComponent<Animator>();

        float explosionDuration = 0f;

        // If the explosion has an Animator, get the duration of the current animation state
        if (explosionAnimator != null)
        {
            explosionDuration = explosionAnimator.GetCurrentAnimatorStateInfo(0).length;
        }

        // Wait for the explosion animation to finish
        yield return new WaitForSeconds(explosionDuration);

        // Check if the explosion GameObject is still valid before destroying it
        if (explosion != null)
        {
            // Destroy the explosion GameObject to clean up after the effect has finished
            Destroy(explosion);
        }
    }


    void RotateTowardsPlayer()
    {
        // Calculate the direction vector from the object to the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Calculate the angle in degrees between the direction vector and the x-axis
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply a 90-degree offset to align the object's forward direction
        angle -= 90f;

        // Create a target rotation based on the calculated angle
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Smoothly rotate the object towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    void RotateTurretTowardsPlayer()
    {
        // Check if the turret is assigned
        if (turret == null) return;

        // Calculate the direction vector from the turret to the player
        Vector2 direction = (player.position - turret.position).normalized;

        // Calculate the angle in degrees between the direction vector and the x-axis
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply a 180-degree offset to align the turret's forward direction
        angle -= 180f;

        // Create a target rotation based on the calculated angle
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Smoothly rotate the turret towards the target rotation
        turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRotation, turretRotationSpeed * Time.deltaTime);
    }


    void RotateTowardsWaypoint()
    {
        // Get the target waypoint's Transform component from the array using the current waypoint index
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Calculate the direction vector from the object's position to the waypoint and normalize it
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;

        // Calculate the angle in degrees needed to face the waypoint
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply a 90-degree offset to align the object's forward direction with the calculated angle
        angle -= 90f;

        // Create a Quaternion representing the target rotation based on the calculated angle
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Smoothly rotate the object towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if the object has nearly reached the target rotation
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            // Set flags to indicate rotation and reorientation are complete
            isRotating = false;
            reorienting = false;
        }
    }


    void MoveTowardsWaypoint()
    {
        // Return if the object is still rotating
        if (isRotating) return;

        // Get the target waypoint's Transform component from the array using the current waypoint index
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Move the object towards the waypoint by a distance proportional to moveSpeed and deltaTime
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Check if the object is close enough to the waypoint to consider it reached
        if (Vector2.Distance(transform.position, targetWaypoint.position) < waypointTolerance)
        {
            // Update to the next waypoint in the array, looping back to the start if necessary
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            // Set flag to indicate that the object needs to start rotating towards the next waypoint
            isRotating = true;
        }
    }


    private void PlayRandomSound(AudioClip[] clips) //Plays audio
    {
        if (clips.Length == 0) return;

        // Pick a random clip
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySoundAtPoint(clip, transform.position);
    }

    private void PlaySoundAtPoint(AudioClip clip, Vector3 position) // Handles audio playback
    {
        GameObject soundGameObject = new GameObject("EnemySFX");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(soundGameObject, clip.length);
    }
}