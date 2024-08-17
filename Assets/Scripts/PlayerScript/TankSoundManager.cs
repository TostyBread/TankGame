using UnityEngine;

public class TankSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip idleSound;        // For default tanks
    public AudioClip revvingSound;     // For default tanks
    public AudioClip specialSound;     // For special tanks

    public float movingPitch = 1.5f;   // Pitch when moving for special tanks
    public float idlePitch = 1.0f;     // Pitch when idle for special tanks
    public float pitchTransitionSpeed = 2.0f; // Speed of pitch transition

    private PlayerController playerController;
    private CameraController cameraController;

    void Start()
    {
        // Find the CameraController in the scene
        cameraController = FindObjectOfType<CameraController>();

        if (cameraController == null)
        {
            //Debug.LogError("CameraController not found in the scene.");
            return;
        }

        // Set the initial PlayerController based on the active tank
        SetPlayerController();

        // Initialize sound settings
        UpdateSoundSettings();
    }
    private void FixedUpdate()
    {
        if (playerController == null)
        {
            audioSource.Stop();
            return;
        }
    }
    void Update()
    {
        if (cameraController == null)
        {
            // Attempt to find CameraController if it's missing
            cameraController = FindObjectOfType<CameraController>();
            if (cameraController == null)
            {
                //Debug.LogError("CameraController not found in the scene.");
                return;
            }
        }

        // Check for missing audio source or player controller
        if (audioSource == null || playerController == null)
        {
            // Try to update PlayerController if necessary
            SetPlayerController();
            return;
        }

        bool isMoving = playerController.IsMoving();

        if (!gameObject.activeInHierarchy)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            return;
        }

        // Check if cameraController.Follow is valid
        if (cameraController.IsFollowingSpecialTank)
        {
            // Proceed with handling special tank sounds
            HandleSpecialTankSound(playerController.IsMoving());
        }
        else
        {
            // Proceed with handling default tank sounds
            HandleDefaultTankSound(playerController.IsMoving());
        }
    }

    private void HandleSpecialTankSound(bool isMoving)
    {
        if (audioSource == null)
        {
            return; // Exit if audioSource is missing
        }


        if (isMoving)
        {
            if (audioSource.clip != specialSound)
            {
                // Stop any currently playing sound and switch to special sound
                audioSource.Stop();
                audioSource.clip = specialSound;
                audioSource.loop = true; // Ensure the sound loops for special tanks
                audioSource.Play();
            }
            audioSource.pitch = Mathf.Lerp(audioSource.pitch, movingPitch, Time.deltaTime * pitchTransitionSpeed);
        }
        else
        {
            audioSource.pitch = Mathf.Lerp(audioSource.pitch, idlePitch, Time.deltaTime * pitchTransitionSpeed);
            if (!audioSource.isPlaying)
            {
                audioSource.clip = specialSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    private void HandleDefaultTankSound(bool isMoving)
    {
        if (audioSource == null)
        {
            return; // Exit if audioSource is missing
        }


        if (isMoving)
        {
            if (audioSource.clip != revvingSound)
            {
                // Stop any currently playing sound and switch to revving sound
                audioSource.Stop();
                audioSource.clip = revvingSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip != idleSound)
            {
                // Stop any currently playing sound and switch to idle sound
                audioSource.Stop();
                audioSource.clip = idleSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        // Set volume to 1 for both idle and revving sounds
        audioSource.volume = 0.8f;
    }

    void SetPlayerController()
    {
        if (cameraController == null) return;

        GameObject activeTank = cameraController.IsFollowingSpecialTank ? cameraController.specialTank : cameraController.defaultTank;

        if (activeTank != null)
        {
            playerController = activeTank.GetComponent<PlayerController>();
        }
        else
        {
            return;
        }
    }


    void UpdateSoundSettings()
    {
        if (cameraController.IsFollowingSpecialTank)
        {
            if (audioSource != null)
            {
                audioSource.clip = specialSound;
                audioSource.loop = true; // Ensure the sound loops for special tanks
            }
        }
        else
        {
            if (audioSource != null)
            {
                audioSource.loop = false; // Default loop setting
            }
        }
    }
}