using UnityEngine;

public class Health : MonoBehaviour
{
    public int hitpoints = 3; // Default hitpoints
    public GameObject destructionEffectPrefab; // Prefab for destruction effect (animation and sound)
    public AudioClip[] destructionSounds; // Array of sounds to play on destruction

    EnemyMovementAndBehaviour enemyMovementAndBehaviour; // Mention EnemyMovementAndBehaviour

    private void Start()
    {
        enemyMovementAndBehaviour = GetComponent<EnemyMovementAndBehaviour>();
    }

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            Die();
            if (enemyMovementAndBehaviour != null)
            {
                enemyMovementAndBehaviour.isDead = true; // Safely refer to enemyMovementAndBehaviour
            }
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

            // Play a random sound effect
            if (destructionSounds.Length > 0)
            {
                AudioClip randomClip = destructionSounds[Random.Range(0, destructionSounds.Length)];
                AudioSource.PlayClipAtPoint(randomClip, transform.position);
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