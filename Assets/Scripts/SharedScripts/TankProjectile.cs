using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProjectile : MonoBehaviour
{
    public float projectileSpeed = 50f; // Projectile speed
    public float projectileLifetime = 5f;

    public AudioClip HitTankSFX;
    public AudioClip HitMiscSFX;
    public AudioClip HitDestructableSFX;

    public bool EnemyProjectile = false; // A toggle to check whether projectile can damage player

    public GameObject hitMisc; // Explosion for misc prefab
    public GameObject hitTank; // Explosion for enemy prefab

    private TilemapHandler tilemapHandler; //Referes to TilemapHandler

    private void Start()
    {
        tilemapHandler = FindObjectOfType<TilemapHandler>(); // Find the tilemap handler
        GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed; // projectile travel trajectory
        Destroy(gameObject, projectileLifetime); // initiate projectile lifetime
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !EnemyProjectile) // When hit enemy
        {
            PlayHitAnimation(hitTank, collision.transform.position);
            Destroy(collision.gameObject);
            //PlaySoundAtPoint(HitEnemySFX, transform.position);
            Destroy(gameObject);
        }
        if (collision.CompareTag("Player") && EnemyProjectile) // When hit enemy
        {
            PlayHitAnimation(hitTank, collision.transform.position);
            Destroy(collision.gameObject);
            //PlaySoundAtPoint(HitTankSFX, transform.position);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Destroyable")) // When hit destructable object
        {
            // Only remove the specific tile hit by the projectile
            tilemapHandler.RemoveTile(transform.position);
            //PlaySoundAtPoint(HitDestructableSFX, transform.position);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacles")) // When hit non destructable object
        {
            PlayHitAnimation(hitMisc, transform.position);
            //PlaySoundAtPoint(HitMiscSFX, transform.position);
            Destroy(gameObject);
        }
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
