using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProjectileExperimental : MonoBehaviour
{
    public float projectileSpeed = 50f; // Projectile speed
    public float projectileLifetime = 5f;
    public float ricochetAngleThreshold = 20f; // Angle threshold for ricochet

    public AudioClip HitEnemySFX;
    public AudioClip HitMiscSFX;
    public AudioClip HitDestructableSFX;

    public GameObject hitMisc; // Explosion for misc prefab
    public GameObject hitEnemy; // Explosion for enemy prefab

    private TilemapHandler tilemapHandler; // References to TilemapHandler
    private Rigidbody2D rb;

    private void Start()
    {
        tilemapHandler = FindObjectOfType<TilemapHandler>(); // Find the tilemap handler
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * projectileSpeed; // Projectile travel trajectory
        Destroy(gameObject, projectileLifetime); // Initiate projectile lifetime
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy")) // When hit enemy
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            Vector2 incomingDirection = -rb.velocity.normalized;

            // Apply 90-degree correction to the collision normal if necessary
            collisionNormal = Quaternion.Euler(-0, -0, -0) * collisionNormal;

            // Calculate the angle between the incoming direction and the collision normal
            float angle = Vector2.Angle(incomingDirection, collisionNormal);
            Debug.Log($"Collision Normal: {collisionNormal}, Incoming Direction: {incomingDirection}, Angle: {angle}");

            if (angle <= ricochetAngleThreshold)
            {
                // Perform ricochet
                Ricochet(collisionNormal);
            }
            else
            {
                // Handle non-ricochet case
                PlayHitAnimation(hitEnemy, collision.transform.position);
                Destroy(gameObject);
            }
        }
        else if (collision.collider.CompareTag("Destroyable")) // When hit destructible object
        {
            tilemapHandler.RemoveTile(transform.position);
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Obstacles")) // When hit non-destructible object
        {
            PlayHitAnimation(hitMisc, transform.position);
            Destroy(gameObject);
        }
    }

    private void Ricochet(Vector2 collisionNormal)
    {
        // Apply a correction to the collision normal if needed
        Vector2 adjustedNormal = Quaternion.Euler(0, 0, 0) * collisionNormal;

        // Reflect the velocity around the adjusted collision normal
        Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, adjustedNormal);
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

    private void PlaySoundAtPoint(AudioClip clip, Vector3 position) // Handles audio playback
    {
        GameObject soundGameObject = new GameObject("WeaponSFX");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(soundGameObject, clip.length);
    }
}


