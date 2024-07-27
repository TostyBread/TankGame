using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Tank speed
    public float rotationSpeed = 200f; // Rotation speed

    void FixedUpdate()
    {
        PlayerMovement();
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
}

