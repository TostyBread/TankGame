using UnityEngine;

public class DynamicAmbientSFX : MonoBehaviour
{
    public AudioSource ambientAudioSource; // Reference to the AudioSource component
    public float maxDistance = 20f; // Maximum distance at which sound is heard
    public float minVolume = 0.1f; // Minimum volume level
    public GameObject defaultTank; // Reference to the default tank GameObject
    public GameObject specialTank; // Reference to the special tank GameObject

    private Transform currentPlayerTransform; // Reference to the current player tank's transform

    void Start()
    {
        // Initialize with the default tank
        SetCurrentPlayerTransform(defaultTank);
    }

    void Update()
    {
        if (currentPlayerTransform != null)
        {
            // Calculate the distance between the player and the sound source
            float distance = Vector3.Distance(currentPlayerTransform.position, transform.position);

            // Calculate the volume based on the distance
            float volume = Mathf.Clamp01(1 - (distance / maxDistance));

            // Ensure volume is not less than the minimum volume
            volume = Mathf.Max(volume, minVolume);

            // Apply the volume to the AudioSource
            if (ambientAudioSource != null)
            {
                ambientAudioSource.volume = volume;
            }
        }
    }

    // Call this method when switching tanks
    public void UpdatePlayerTank(GameObject newPlayerTank)
    {
        SetCurrentPlayerTransform(newPlayerTank);
    }

    private void SetCurrentPlayerTransform(GameObject playerTank)
    {
        if (playerTank != null)
        {
            currentPlayerTransform = playerTank.transform;
        }
        else return;
    }
}
