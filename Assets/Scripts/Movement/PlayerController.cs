using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Tank speed
    public float rotationSpeed = 200f; // Rotation speed

    public GameObject tankProjectile; // Tank projectile
    public float projectileSpeed = 50f; // Tank projectile speed
    public Transform firePos; // Projectile spawn location

    public GameObject explosionPrefab; // Explosion prefab

    void Update()
    {
        PlayerMovement();
        if (Input.GetButtonDown("Fire1"))
        {
            PlayerShoot();
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

    private void PlayerShoot() //Player fires their gun
    {
        Instantiate(tankProjectile, firePos.position, firePos.rotation);
        PlayExplosion();
    }

    //private void PlayExplosion()
    //{
    //    // Create a Quaternion for the 90-degree rotation on the Z-axis
    //    Quaternion explosionRotation = Quaternion.Euler(0, 0, -90);

    //    // Define the offset distance
    //    Vector3 offset = new Vector3(0, 1f, 0); // Adjust the offset as needed

    //    // Calculate the new position with the offset
    //    Vector3 explosionPosition = firePos.position + firePos.right * offset.x + firePos.up * offset.y + firePos.forward * offset.z;

    //    // Instantiate the explosion with the new position and rotation
    //    GameObject explosion = Instantiate(explosionPrefab, explosionPosition, explosionRotation);

    //    // Get the Animator component and find the duration of the explosion animation
    //    Animator explosionAnimator = explosion.GetComponent<Animator>();
    //    float explosionDuration = explosionAnimator.GetCurrentAnimatorStateInfo(0).length;

    //    // Start the coroutine to destroy the explosion after the animation ends
    //    StartCoroutine(DestroyAfterAnimation(explosion, explosionDuration));
    //}

    private void PlayExplosion()
    {
        // Create a Quaternion for the 90-degree rotation on the Z-axis
        Quaternion explosionRotation = Quaternion.Euler(0, 0, 90);

        // Instantiate the explosion with the new rotation
        GameObject explosion = Instantiate(explosionPrefab, firePos.position, explosionRotation);

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

