using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Tank speed
    public float rotationSpeed = 200f; // Hull rotation speed
    public float turretRotationSpeed = 5f; // Turret rotation speed

    public GameObject tankProjectile; // Tank projectile
    public float projectileSpeed = 50f; // Tank projectile speed
    public Transform firePos; // Projectile spawn location
    public Transform turret; // Reference to the tank's turret

    public GameObject explosionPrefab; // Explosion prefab

    void Update()
    {
        PlayerMovement();
        TurretRotation();

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

    private void PlayerShoot() //Player fires their gun
    {
        Instantiate(tankProjectile, firePos.position, firePos.rotation);
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
        Vector3 offset = new Vector3(0, 0.5f, 0); // Adjust the offset as needed

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
}

