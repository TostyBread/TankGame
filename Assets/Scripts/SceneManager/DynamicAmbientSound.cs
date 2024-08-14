using System.Collections;
using UnityEngine;

public class DynamicAmbientSound : MonoBehaviour
{
    public AudioSource ambientAudioSource;  // Reference to the AudioSource component
    [SerializeField] private Transform playerTransform;        // Reference to the player's transform
    public float maxDistance = 20f;          // Maximum distance at which the sound is audible
    public float minVolume = 0.1f;           // Minimum volume level


    void FixedUpdate()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (playerTransform != null)
        {
            // Calculate the distance between the player and the ambient sound source
            float distance = Vector2.Distance(playerTransform.position, transform.position);

            // Calculate volume based on distance
            float volume = Mathf.Clamp01(1 - (distance / maxDistance));

            // Ensure the volume does not go below the minimum volume
            volume = Mathf.Max(volume, minVolume);

            // Apply the calculated volume to the AudioSource
            ambientAudioSource.volume = volume;
        }
    }

}
