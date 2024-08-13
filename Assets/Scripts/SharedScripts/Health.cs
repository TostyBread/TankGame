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

    public void TakeDamage(int damage) // When getting hit by projectile
    {
        hitpoints -= damage; // hitpoint minus by projectile damage
        if (hitpoints <= 0) // if hitpoint reaches zero
        {
            Die();
            if (enemyMovementAndBehaviour != null) // if enemyMovementAndBehaviour still hasn't disable
            {
                enemyMovementAndBehaviour.isDead = true; // make sure enemyMovementAndBehaviour will not run again
            }
        }
    }
    private void Die() 
    {
        if (destructionEffectPrefab != null) // if destructionEffectPrefab is available
        {
            // Assign the gameObject with destructionEffectPrefab and its transform position
            GameObject effect = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity); 

            Animator animator = effect.GetComponent<Animator>(); // grabs animator 
            if (animator != null) // if animator is available
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
                AudioSource.PlayClipAtPoint(randomClip, transform.position); // Plays audio at the location
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