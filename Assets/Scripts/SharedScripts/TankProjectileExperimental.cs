using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProjectileExperimental : MonoBehaviour
{
    public float projectileSpeed = 50f; // Projectile speed
    public float projectileLifetime = 5f;
    public float ricochetAngleThreshold = 70f; // Angle threshold for ricochet

    public AudioClip[] hitTankSFX;
    public AudioClip[] ricochetSFX;
    public AudioClip[] hitMiscSFX;
    public AudioClip[] hitDestructableSFX;

    public GameObject hitMisc; // Explosion for misc prefab
    public GameObject hitTank; // Explosion for enemy prefab

    public bool EnemyProjectile = false; // A toggle to check whether projectile can damage player

    private TilemapHandler tilemapHandler; // References to TilemapHandler
    private Rigidbody2D rb;
    private Collider2D coll; // Collider2D reference

    private void Start()
    {
        // Get the necessary components
        tilemapHandler = FindObjectOfType<TilemapHandler>(); // Find the tilemap handler
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>(); // Get the Collider2D component
        rb.velocity = transform.up * projectileSpeed; // Set projectile velocity
        Destroy(gameObject, projectileLifetime); // Destroy the projectile after its lifetime
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && !EnemyProjectile) // When hit enemy
        {
            HandleCollision(collision, hitTankSFX, hitTank, 1); // Assuming 1 damage points
        }
        else if (collision.collider.CompareTag("Player") && EnemyProjectile) // When hit player
        {
            HandleCollision(collision, hitTankSFX, hitTank, 1); // Assuming 1 damage points
        }
        else if (collision.collider.CompareTag("Destroyable")) // When hit destructible object
        {
            tilemapHandler.RemoveTile(transform.position);
            PlayRandomSound(hitDestructableSFX);
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Obstacles")) // When hit non-destructible object
        {
            PlayHitAnimation(hitMisc, transform.position);
            PlayRandomSound(hitMiscSFX);
            Destroy(gameObject);
        }
    }

    private void HandleCollision(Collision2D collision, AudioClip[] sfxArray, GameObject hitPrefab, int damage)
    {
        var contact = collision.GetContact(0);
        Vector2 collisionNormal = contact.normal;
        Vector2 incomingDirection = -rb.velocity.normalized;

        float angle = Vector2.Angle(incomingDirection, collisionNormal);

        // Debug.Log($"Collision Normal: {collisionNormal}, Incoming Direction: {incomingDirection}, Angle: {angle}"); // Debug on impact angle

        if (angle >= ricochetAngleThreshold)
        {
            // Perform ricochet
            Ricochet(collisionNormal);
            PlayRandomSound(ricochetSFX);
        }
        else
        {
            // Handle non-ricochet case
            PlayHitAnimation(hitPrefab, collision.transform.position);
            PlayRandomSound(sfxArray);

            // Apply damage if the hit object has a Health component
            Health health = collision.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }

    private void Ricochet(Vector2 collisionNormal)
    {
        if (coll != null)
        {
            // Set collider to trigger mode
            coll.isTrigger = true;
        }

        Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, collisionNormal);
        rb.velocity = reflectedVelocity;

        // Optional: Adjust the speed of the ricochet (e.g., reduce speed)
        rb.velocity *= 0.8f; // Reduce speed by 20%
    }

    private void PlayHitAnimation(GameObject hitPrefab, Vector3 position)
    {
        if (hitPrefab != null)
        {
            // Instantiate the hit animation prefab at the impact position
            GameObject hitAnimation = Instantiate(hitPrefab, position, Quaternion.identity);
            // Destroy the animation after its duration
            Animator animator = hitAnimation.GetComponent<Animator>();
            if (animator != null)
            {
                float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
                Destroy(hitAnimation, animationDuration);
            }
            else
            {
                // If no Animator is found, destroy immediately
                Destroy(hitAnimation);
            }
        }
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        // Pick a random clip
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySoundAtPoint(clip, transform.position);
    }

    private void PlaySoundAtPoint(AudioClip clip, Vector3 position)
    {
        GameObject soundGameObject = new GameObject("WeaponSFX");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(soundGameObject, clip.length);
    }
}