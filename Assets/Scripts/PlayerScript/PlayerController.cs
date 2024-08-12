using System.Collections;
using UnityEngine;
using TMPro;

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

    public TextMeshProUGUI reloadText; // Reference to the UI text component
    private bool isReloading = false; // Track reloading status

    public float cooldownTime = 2f; // Cooldown time between shots
    private float cooldownTimer = 0f; // Timer to track cooldown

    // Sound Effects
    public AudioClip[] shootSFX; // tank shooting sfx
    public AudioClip[] reloadedSFX; // tank reloaded sfx
    public AudioClip movementSFX; // tank movement sfx

    private bool wasMoving = false; // Track if the tank was moving last frame
    private AudioSource movementAudioSource; // AudioSource for movement sound

    private void Start()
    {
        reloadText.text = "Ready"; // Hide the reloading text during start

        // Initialize the AudioSource for movement sound
        movementAudioSource = gameObject.AddComponent<AudioSource>();
        movementAudioSource.clip = movementSFX;
        movementAudioSource.loop = true; // Set to loop so we can control playback easily
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
                isReloading = true; // Start reloading
            }
        }

        // Update the reloading text based on the reloading status
        if (isReloading)
        {
            reloadText.text = "Reloading...";
            if (cooldownTimer <= 0f)
            {
                isReloading = false; // Reloading complete
                reloadText.text = "Ready"; // Hide the reloading text
                PlayRandomSound(reloadedSFX); // Play reloaded sound effect
            }
        }
    }

    public bool IsMoving() // Reference to TankEngineSFX
    {
        float move = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        return Mathf.Abs(move) > 0 || Mathf.Abs(rotate) > 0;
    }

    private void PlayerMovement()
    {
        // Handles tank move and turn control
        float move = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        // Use transform 
        transform.Translate(0, move, 0);
        transform.Rotate(0, 0, -rotate);

        // Determine if the tank is moving
        bool isMoving = Mathf.Abs(move) > 0 || Mathf.Abs(rotate) > 0;

        // Play movement sound if moving and not already playing
        if (isMoving && !wasMoving)
        {
            movementAudioSource.Play(); // Start the movement sound
        }

        // Stop movement sound if not moving and was playing
        if (!isMoving && wasMoving)
        {
            movementAudioSource.Stop(); // Stop the movement sound
        }

        // Update the flag
        wasMoving = isMoving;
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

    private void PlayerShoot()
    {
        Instantiate(tankProjectile, firePos.position, firePos.rotation);
        PlayRandomSound(shootSFX);
        PlayExplosion();
    }

    private void PlayExplosion()
    {
        // Get the player's current rotation
        Quaternion turretRotation = turret.rotation;

        // Define the fixed rotation (90 degrees on the Z-axis)
        Quaternion fixedRotation = Quaternion.Euler(0, 0, 180);

        // Combine the player's rotation with the fixed rotation
        Quaternion combinedRotation = turretRotation * fixedRotation;

        // Define the offset distance
        Vector3 offset = new Vector3(0, 0.9f, 0); // Adjust the offset as needed

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

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        // Pick a random clip
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySoundAtPoint(clip, transform.position);
    }

    private void PlaySoundAtPoint(AudioClip clip, Vector3 position) // Handles audio playback
    {
        GameObject soundGameObject = new GameObject("PlayerSFX");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(soundGameObject, clip.length);
    }

    private void OnDestroy()
    {
        if (reloadText != null)
        {
            // Disable the reloadText game object
            reloadText.text = "";
        }
    }
}
