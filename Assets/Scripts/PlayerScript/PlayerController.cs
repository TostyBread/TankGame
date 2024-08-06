using System.Collections;
using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Tank speed
    public float rotationSpeed = 50f; // Hull rotation speed
    public float turretRotationSpeed = 5f; // Turret rotation speed

    public GameObject tankProjectile; // Tank projectile
    public float projectileSpeed = 50f; // Tank projectile speed

    public Transform firePos; // Projectile spawn location
    public Transform turret; // Reference to the tank's turret

    public GameObject explosionPrefab; // Explosion prefab

    public float cooldownTime = 2f; // Cooldown time between shots
    private float cooldownTimer = 0f; // Timer to track cooldown

    public TMP_Text reloadingText; // TextMeshPro UI element to display reloading message

    void Start()
    {
        if (reloadingText != null)
        {
            reloadingText.enabled = false; // Ensure the text is initially hidden
        }
    }

    void Update()
    {
        PlayerMovement();
        TurretRotation();

        // Update the cooldown timer
        cooldownTimer -= Time.deltaTime;

        // Check for shooting input and cooldown
        if (Input.GetButtonDown("Fire1"))
        {
            if (cooldownTimer <= 0f)
            {
                PlayerShoot();
                // Reset the cooldown timer
                cooldownTimer = cooldownTime;
                if (reloadingText != null)
                {
                    reloadingText.enabled = false; // Hide reloading text when shooting
                }
            }
            else
            {
                // Show reloading message
                if (reloadingText != null)
                {
                    reloadingText.enabled = true; // Show reloading text
                }
            }
        }
        else
        {
            // Hide reloading text when button is not pressed
            if (reloadingText != null)
            {
                reloadingText.enabled = false;
            }
        }
    }

    private void PlayerMovement()
    {
        // Handles tank move and turn control
        float move = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        // Uses transform 
        transform.Translate(0, move, 0);
        transform.Rotate(0, 0, -rotate);
    }

    private void TurretRotation()
    {
        // Get the mouse position in world space
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ensure z is zero for 2D

        // Calculate the direction from the turret to the mouse
        Vector3 direction = mouseWorldPosition - turret.position;

        // Calculate the target angle in radians
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly rotate the turret towards the target angle
        float angle = Mathf.LerpAngle(turret.eulerAngles.z, targetAngle, Time.deltaTime * turretRotationSpeed);
        turret.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void PlayerShoot() // Player fires their gun
    {
        Instantiate(tankProjectile, firePos.position, firePos.rotation);
        PlayExplosion();
    }

    private void PlayExplosion() // Plays explosion
    {
        // Get the player's current rotation
        Quaternion turretRotation = turret.rotation;

        // Define the fixed rotation (90 degrees on the Z-axis)
        Quaternion fixedRotation = Quaternion.Euler(0, 0, 180);

        // Combine the player's rotation with the fixed rotation
        Quaternion combinedRotation = turretRotation * fixedRotation;

        // Define the offset distance
        Vector3 offset = new Vector3(0, 0.5f, 0); // Adjust the offset as needed

        // Calculate the new position with the offset
        Vector3 explosionPosition = firePos.position + firePos.right * offset.x + firePos.up * offset.y + firePos.forward * offset.z;

        // Instantiate the explosion with the combined rotation
        GameObject explosion = Instantiate(explosionPrefab, explosionPosition, combinedRotation);

        // Get the Animator component and find the duration of the explosion animation
        Animator explosionAnimator = explosion.GetComponent<Animator>();
        float explosionDuration = explosionAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Start the coroutine to destroy the explosion after the animation ends
        StartCoroutine(DestroyAfterAnimation(explosion, explosionDuration));
    }

    private IEnumerator DestroyAfterAnimation(GameObject explosion, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(explosion);
    }
}

