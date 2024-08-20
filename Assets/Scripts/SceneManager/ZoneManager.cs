using UnityEngine;
using TMPro; // Import the TextMeshPro namespace for UI text elements

public class ZoneManager : MonoBehaviour
{
    [System.Serializable]
    public class Zone
    {
        public GameObject zoneObject; // GameObject representing the zone (e.g., a visual area in the game)
        public GameObject[] enemyTanks; // Array to hold enemy tanks within this zone
        public float xMin; // Minimum x-axis value defining the left boundary of the zone
        public float xMax; // Maximum x-axis value defining the right boundary of the zone
    }

    public Zone[] zones; // Array to hold the different zones in the game
    public GameObject defaultTank; // Reference to the default player tank
    public GameObject specialTank; // Reference to any special player tank or secondary player
    public TextMeshProUGUI warningText; // UI TextMeshPro element to display warnings to the player
    public GameObject warningTextUI; // GameObject for the warning text
    public TextMeshProUGUI completionText; // UI TextMeshPro element to display completion messages
    public GameObject completionTextUI; // GameObject for the completion text
    public float returnTime = 5f; // Time allowed for the player to return to the zone before being penalized
    public float completionDisplayTime = 3f; // Time to display the completion message after clearing a zone
    public GameObject ArrowUI; // Arrow UI when player cleared the zone

    // Audio Sources and Clips
    public AudioSource audioSource; // AudioSource for playing sound effects
    public AudioClip completionSound; // Sound effect for zone completion
    public AudioClip victorySound; // Sound effect for game victory

    private int currentZoneIndex = 0; // Index of the currently active zone
    private bool isWarningActive = false; // Flag to indicate if the warning message is currently active
    private bool isCompletionMessageActive = false; // Flag to indicate if the completion message is currently active
    private float warningTimer = 0f; // Timer for tracking the duration of the warning message
    private float completionTimer = 0f; // Timer for tracking the duration of the completion message
    private bool zoneCleared = false; // Flag to indicate if the current zone has been cleared
    private bool givenDamage = false; // Flag to ensure damage is only given once

    UIManager uIManager; // Reference to the UIManager for managing UI elements

    void Start()
    {
        uIManager = GetComponent<UIManager>(); // Get the UIManager component
        ArrowUI.SetActive(false); // Hide arrow UI at start
        // Hide text initially
        completionTextUI.SetActive(false);
        warningTextUI.SetActive(false);

        // Initialize all zones to inactive
        foreach (Zone zone in zones)
        {
            zone.zoneObject.SetActive(false);
        }
        // Activate the first zone
        if (zones.Length > 0)
        {
            zones[0].zoneObject.SetActive(true);
        }
        warningText.gameObject.SetActive(false);
        completionText.gameObject.SetActive(false);

        // Get the AudioSource component if not assigned in the Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // If player tanks are not assigned, do nothing
        if (defaultTank == null || specialTank == null) return;

        // Process only if within valid zone index
        if (currentZoneIndex < zones.Length)
        {
            CheckPlayerPosition(); // Check if the player is within the current zone

            // Check if all enemies in the current zone are destroyed
            if (AreAllEnemiesDestroyed())
            {
                if (!zoneCleared)
                {
                    // Handle completion message and zone activation
                    if (currentZoneIndex == zones.Length - 1)
                    {
                        ShowAreaSecuredMessage(); // Final zone cleared
                    }
                    else
                    {
                        ShowCompletionMessage(); // Intermediate zone cleared
                    }
                    zoneCleared = true;
                }
            }
            else
            {
                // Reset zone cleared flag if enemies are not cleared yet
                zoneCleared = false;
            }

            // Handle the display of the completion message
            if (isCompletionMessageActive)
            {
                completionTimer += Time.deltaTime; // Update the timer
                if (completionTimer >= completionDisplayTime)
                {
                    // Hide completion message after the display time
                    completionText.gameObject.SetActive(false);
                    completionTextUI.SetActive(false);
                    isCompletionMessageActive = false;
                    ArrowUI.SetActive(false);
                    completionTimer = 0f;

                    // Activate the next zone
                    ActivateNextZone();
                    completionText.gameObject.SetActive(false);
                }
            }
        }
    }

    void CheckPlayerPosition()
    {
        if (zones.Length == 0) return; // Exit if no zones are defined

        Zone currentZone = zones[currentZoneIndex]; // Get the current zone
        float playerX = defaultTank.transform.position.x; // Get the x position of the default tank
        float playerXtoo = specialTank.transform.position.x; // Get the x position of the special tank

        // Check if either tank is outside the current zone boundaries
        if (playerX < currentZone.xMin || playerX > currentZone.xMax || playerXtoo < currentZone.xMin || playerXtoo > currentZone.xMax)
        {
            if (!isWarningActive)
            {
                // Start the warning sequence if not already active
                warningText.gameObject.SetActive(true);
                warningTextUI.SetActive(true);
                isWarningActive = true;
            }

            // Update the warning timer
            warningTimer += Time.deltaTime;
            warningText.text = $"Return to the combat area! {Mathf.Max(returnTime - warningTimer, 0):F1}";

            if (warningTimer >= returnTime)
            {
                // If the player did not return in time, apply damage
                Health health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
                if (health != null && !givenDamage)
                {
                    health.TakeDamage(50); // Apply damage
                    givenDamage = true;
                }
            }
        }
        else
        {
            // Player is within the zone limits, reset warning
            if (isWarningActive)
            {
                warningText.gameObject.SetActive(false);
                warningTextUI.SetActive(false);
                isWarningActive = false;
                warningTimer = 0f;
            }
        }
    }

    bool AreAllEnemiesDestroyed()
    {
        // Check if all enemies in the current zone are destroyed
        Zone currentZone = zones[currentZoneIndex];
        foreach (GameObject enemy in currentZone.enemyTanks)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                return false; // An enemy is still in the zone
            }
        }
        return true; // All enemies are destroyed
    }

    void ActivateNextZone()
    {
        // Deactivate the current zone
        if (currentZoneIndex < zones.Length)
        {
            zones[currentZoneIndex].zoneObject.SetActive(false);
        }
        // Activate the next zone or show victory message
        if (currentZoneIndex + 1 < zones.Length)
        {
            currentZoneIndex++;
            zones[currentZoneIndex].zoneObject.SetActive(true);
        }
        else
        {
            // All zones cleared, show victory message
            ShowVictoryScreen();
        }
    }

    void ShowCompletionMessage()
    {
        if (currentZoneIndex < zones.Length - 1) // Intermediate zones
        {
            completionText.gameObject.SetActive(true);
            completionTextUI.SetActive(true);
            completionText.text = "All enemies in this area have been eliminated, advance!";
            isCompletionMessageActive = true;
            PlaySound(completionSound);
        }
    }

    void ShowAreaSecuredMessage()
    {
        completionText.gameObject.SetActive(true);
        completionTextUI.SetActive(true);
        ArrowUI.SetActive(false);
        completionText.text = "All enemies have been destroyed, we've secured the area!";
        isCompletionMessageActive = true;
        PlaySound(completionSound);
    }

    void ShowVictoryScreen()
    {
        uIManager.isTimerRunning = false; // Stop the timer
        uIManager.UpdateTimerText();      // Ensure the timer text is up-to-date
        uIManager.victoryPanel.SetActive(true); // Show the victory panel
        PlaySound(victorySound);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip); // Play the specified sound effect
        }
    }
}
