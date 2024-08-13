using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProjectileExperimental : MonoBehaviour
{
    public float projectileSpeed = 50f; // Projectile speed
    public float projectileLifetime = 5f; // Projectile life time
    public float ricochetAngleThreshold = 70f; // Angle threshold for ricochet

    public AudioClip[] hitTankSFX; // Sound for tank hit
    public AudioClip[] ricochetSFX; // Sound for ricochet
    public AudioClip[] hitMiscSFX; // Sound for hitting other stuff
    public AudioClip[] hitDestructableSFX; // Sound for hitting destructable object

    public GameObject hitMisc; // Explosion for misc prefab
    public GameObject hitTank; // Explosion for enemy prefab

    public bool EnemyProjectile = false; // A toggle to check whether projectile can damage player
    public int shellDamage = 1; // Damage input here

    private TilemapHandler tilemapHandler; // References to TilemapHandler
    private Rigidbody2D rb; // Rigidbody2D reference
    private Collider2D coll; // Collider2D reference

    private void Start()
    {
        // Get the necessary components
        tilemapHandler = FindObjectOfType<TilemapHandler>(); // Find the tilemap handler
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        coll = GetComponent<Collider2D>(); // Get the Collider2D component
        rb.velocity = transform.up * projectileSpeed; // Set projectile velocity
        Destroy(gameObject, projectileLifetime); // Destroy the projectile after its lifetime
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && !EnemyProjectile) // When hit enemy
        {
            HandleCollision(collision, hitTankSFX, hitTank, shellDamage); // transfer to that collider (Player) with SFX, animation & damage
        }
        else if (collision.collider.CompareTag("Player") && EnemyProjectile) // When hit player
        {
            HandleCollision(collision, hitTankSFX, hitTank, shellDamage); // transfer to that collider (Player) with SFX, animation & damage
        }
        else if (collision.collider.CompareTag("Destroyable")) // When hit destructible object
        {
            Vector3 contactPoint = collision.contacts[0].point; // contactPoint will store the collision contact point and location
            tilemapHandler.RemoveTile(contactPoint); // tells the tilemap handler to remove that specific tile where contacts are made
            PlayRandomSound(hitDestructableSFX); // Plays the sound
            Destroy(gameObject); // destroy the projectile
        }
        else if (collision.collider.CompareTag("Obstacles")) // When hit non-destructible object
        {
            PlayHitAnimation(hitMisc, transform.position); // Plays the animation
            PlayRandomSound(hitMiscSFX); // Plays the sound
            Destroy(gameObject); // destroy the projectile
        }
    }

    private void HandleCollision(Collision2D collision, AudioClip[] sfxArray, GameObject hitPrefab, int damage)
    {
        var contact = collision.GetContact(0); // GetContact retrieves the first contact point
        Vector2 collisionNormal = contact.normal; // retrieves the normal vector of the collision contact point (vector perpendicular to surface collision)
        Vector2 incomingDirection = -rb.velocity.normalized; // determines direction of the incoming projectile before the collision

        float angle = Vector2.Angle(incomingDirection, collisionNormal); // Calculates the angle between incoming direction and collision normal with Vector2.Angle

        // Debug.Log($"Collision Normal: {collisionNormal}, Incoming Direction: {incomingDirection}, Angle: {angle}"); // Debug on impact angle

        if (angle >= ricochetAngleThreshold) // if angle of impact is steeper than treshold
        {
            // Perform ricochet
            Ricochet(collisionNormal);
            PlayRandomSound(ricochetSFX);
        }
        else
        {
            // Handle non-ricochet case
            PlayHitAnimation(hitPrefab, collision.transform.position); // Plays animation
            PlayRandomSound(sfxArray); // Plays a sound

            // Apply damage if the hit object has a Health component
            Health health = collision.collider.GetComponent<Health>();
            if (health != null) // if target health is available
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject); // destroy the projectile
        }
    }

    private void Ricochet(Vector2 collisionNormal)
    {
        if (coll != null)
        {
            // Set collider to trigger mode so that it wont deal damage anymore
            coll.isTrigger = true;
        }

        Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, collisionNormal); // Vector2.Reflect handles reflection calculation
        rb.velocity = reflectedVelocity;

        // Speed of the ricochet (e.g., reduce speed)
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