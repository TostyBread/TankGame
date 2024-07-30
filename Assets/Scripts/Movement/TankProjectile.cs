using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProjectile : MonoBehaviour
{
    public float projectileSpeed = 50f; // Projectile speed
    public float projectileLifetime = 5f;

    private float despawnTimer;


    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed;
        despawnTimer = projectileLifetime; // Assign the lifetime everytime a projectile is fired
    }
    
    //void Update()
    //{
        
    //    ProjectileDespawn();
    //}

    private void ProjectileDespawn()
    {
        if (projectileLifetime > 0) // if there's still time in projectile, decreae it relative to Time.deltaTime
        {
            despawnTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
