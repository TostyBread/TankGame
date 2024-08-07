using UnityEngine;

public class Health : MonoBehaviour
{
    public int hitpoints = 3; // Default hitpoints
    public GameObject destructionEffectPrefab; // Prefab for destruction effect (animation and sound)
    public AudioClip destructionSound; // Sound to play on destruction

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (destructionEffectPrefab != null)
        {
            GameObject effect = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);

            Animator animator = effect.GetComponent<Animator>();
            if (animator != null)
            {
                // Use the animation length to wait before destroying the effect
                float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                // Destroy the effect after the animation completes
                Destroy(effect, animationLength);
            }
            else
            {
                // If no Animator is found, destroy immediately
                Destroy(effect);
            }

            // Play sound effect
            if (destructionSound != null)
            {
                AudioSource.PlayClipAtPoint(destructionSound, transform.position);
            }

            // Destroy the GameObject after a short delay to ensure effects play
            Destroy(gameObject, Mathf.Max(0.1f, animator != null ? animator.GetCurrentAnimatorStateInfo(0).length : 0.1f));
        }
        else
        {
            // Destroy immediately if no effect is present
            Destroy(gameObject);
        }
    }
}
