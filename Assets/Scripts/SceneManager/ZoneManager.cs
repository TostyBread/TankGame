using UnityEngine;
using TMPro; // Import TMPro namespace for TextMeshPro

public class ZoneManager : MonoBehaviour
{
    [System.Serializable]
    public class Zone
    {
        public GameObject zoneObject; // GameObject representing the zone
        public GameObject[] enemyTanks; // Array to hold enemy tanks for this zone
        public float xMin; // Minimum x-axis value for this zone
        public float xMax; // Maximum x-axis value for this zone
    }

    public Zone[] zones; // Array to hold the different zones
    public GameObject defaultTank; // Reference to the player
    public GameObject specialTank;
    public TextMeshProUGUI warningText; // UI TextMeshPro to display warnings
    public GameObject warningTextShade; 
    public TextMeshProUGUI completionText; // UI TextMeshPro to display completion messages
    public GameObject completionTextShade;
    public float returnTime = 5f; // Time allowed for player to return to the zone before being destroyed
    public float completionDisplayTime = 3f; // Time to display the completion message

    // Audio Sources and Clips
    public AudioSource audioSource;
    public AudioClip completionSound;
    public AudioClip victorySound;

    private int currentZoneIndex = 0; // Index of the currently active zone
    private bool isWarningActive = false;
    private bool isCompletionMessageActive = false;
    private float warningTimer = 0f;
    private float completionTimer = 0f;
    private bool zoneCleared = false;
    private bool givenDamage = false;

    UIManager uIManager;

    void Start()
    {
        uIManager = GetComponent<UIManager>(); // Reference UIManager

        completionTextShade.SetActive(false);
        warningTextShade.SetActive(false);

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

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Reference AudioSource if not assigned in the Inspector
        }
    }

    void Update()
    {
        if (defaultTank == null || specialTank == null) return; // When player died, do nothing

        if (currentZoneIndex < zones.Length)
        {
            CheckPlayerPosition();

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
                zoneCleared = false; // Reset flag if enemies are not cleared yet

            }

            if (isCompletionMessageActive)
            {
                // Update the completion message timer
                completionTimer += Time.deltaTime;
                if (completionTimer >= completionDisplayTime)
                {
                    // Hide completion message after the display time
                    completionText.gameObject.SetActive(false);
                    completionTextShade.SetActive(false);
                    isCompletionMessageActive = false;
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
        if (zones.Length == 0) return;

        Zone currentZone = zones[currentZoneIndex];
        float playerX = defaultTank.transform.position.x;
        float playerXtoo = specialTank.transform.position.x;

        if (playerX < currentZone.xMin || playerX > currentZone.xMax || playerXtoo < currentZone.xMin || playerXtoo > currentZone.xMax)
        {
            if (!isWarningActive)
            {
                // Start warning sequence
                warningText.gameObject.SetActive(true);
                warningTextShade.SetActive(true);
                isWarningActive = true;
            }

            // Update warning timer
            warningTimer += Time.deltaTime;
            warningText.text = $"Return to the combat area! {Mathf.Max(returnTime - warningTimer, 0):F1}";

            if (warningTimer >= returnTime)
            {
                // Destroy player if they did not return in time
                // Apply damage if the hit object has a Health component
                Health health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
                if (health != null && !givenDamage)
                {
                    health.TakeDamage(10);
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
                warningTextShade.SetActive(false);
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
            // All zones cleared, show area secured message
            ShowVictoryScreen();
        }
    }

    void ShowCompletionMessage()
    {
        if (currentZoneIndex < zones.Length - 1) // Intermediate zones
        {
            completionText.gameObject.SetActive(true);
            completionTextShade.SetActive(true);
            completionText.text = "All enemies in this area have been eliminated, advance!";
            isCompletionMessageActive = true;
            PlaySound(completionSound);
        }
    }

    void ShowAreaSecuredMessage()
    {
        completionText.gameObject.SetActive(true);
        completionTextShade.SetActive(true);
        completionText.text = "All enemies have been destroyed, we've secured the area!";
        isCompletionMessageActive = true;
        PlaySound(completionSound);
    }

    void ShowVictoryScreen()
    {
        uIManager.isTimerRunning = false; // Stop the timer
        uIManager.UpdateTimerText();      // Ensure the timer text is up-to-date
        uIManager.victoryPanel.SetActive(true); // When player wins, activate the victory panel
        PlaySound(victorySound);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}