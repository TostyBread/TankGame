using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public AudioClip[] explosionSFX;
    public GameObject explosion; // Explosion for enemy prefab

    private Collider2D coll; // Collider2D reference

    private void Start()
    {
        coll = GetComponent<Collider2D>(); // Get the Collider2D component
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            coll.isTrigger = true; // immediately disable trigger to avoid player taking multiple damage over landmine
            HandleCollision(collision, explosionSFX, explosion, 1);
        }
    }

    private void HandleCollision(Collision2D collision, AudioClip[] sfxArray, GameObject hitPrefab, int damage)
    {
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
