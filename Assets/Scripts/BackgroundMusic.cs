using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip[] musicClips; // Array to hold the WAV audio clips
    private AudioSource audioSource; // Reference to the AudioSource component
    public Transform playerLocation; // Reference to the player's Transform
    public GameObject playerTank; // Reference to whether player tank is active in hierarchy
    public float triggerXPosition = 38.0f; // X-axis position to trigger the music start
    [Range(0.0f, 1.0f)] public float volume = 0.5f; // Volume control, range between 0.0 and 1.0

    private int currentClipIndex = 0; // Index to track which clip is currently playing
    private bool musicStarted = false; // Flag to check if music has started

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        foreach (var clip in musicClips) //Pre-load all music data first.
        {
            clip.LoadAudioData();
        }

        if (musicClips.Length > 0)
        {
            audioSource.clip = musicClips[currentClipIndex];
            audioSource.loop = false;
            audioSource.volume = volume; // Set initial volume
        }
    }


    void Update()
    {
        if (playerLocation == null || !playerTank.activeInHierarchy) return; // if player died, ignore it

        // Check if the player has moved past the specified x-axis position
        if (!musicStarted && playerLocation.position.x > triggerXPosition)
        {
            PlayNextClip();
            musicStarted = true;
        }

        // Check if the current audio clip has finished playing
        if (!audioSource.isPlaying && musicStarted)
        {
            PlayNextClip();
        }

        // Update volume if it has changed
        if (audioSource.volume != volume)
        {
            audioSource.volume = volume;
        }
    }

    void PlayNextClip()
    {
        if (musicClips.Length == 0 || !playerTank.activeInHierarchy) return;

        // Move to the next clip
        currentClipIndex = (currentClipIndex + 1) % musicClips.Length;
        audioSource.clip = musicClips[currentClipIndex];
        audioSource.Play();
    }
}
