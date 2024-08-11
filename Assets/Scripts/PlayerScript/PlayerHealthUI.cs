using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    public GameObject playerHealthGameObject; // Reference to the playerHealth GameObject
    public GameObject UIOvershade; // Background shade for UI
    public TextMeshProUGUI hitpointsText; // Reference to the UI text element
    public float flashInterval = 0.5f; // Interval for flashing text
    private Color originalColor;
    private bool isFlashing = false;

    private void Start()
    {
        if (hitpointsText != null)
        {
            originalColor = hitpointsText.color;
        }

        UIOvershade.SetActive(true); // Toggle on shade at start

        UpdateHealthDisplay(); // Initialize health display
    }

    private void Update()
    {
        UpdateHealthDisplay(); // Update health display based on the current tank
    }

    private void UpdateHealthDisplay()
    {
        // Check if playerHealthGameObject is valid and active
        if (playerHealthGameObject == null || !playerHealthGameObject.activeInHierarchy)
        {
            // Deactivate hitpointsText if playerHealthGameObject is null or inactive
            if (hitpointsText != null)
            {
                hitpointsText.gameObject.SetActive(false);
            }
            return; // Exit the method early since there's no need to update the UI
        }

        Health currentHealth = playerHealthGameObject.GetComponent<Health>();

        if (currentHealth == null)
        {
            // Handle the case where the playerHealth component is missing
            if (hitpointsText != null)
            {
                hitpointsText.gameObject.SetActive(false);
            }
            return;
        }

        if (currentHealth.hitpoints <= 0)
        {
            // Hide the text if hitpoints are zero or less
            if (hitpointsText != null)
            {
                hitpointsText.gameObject.SetActive(false);
                UIOvershade.SetActive(false);
            }
        }
        else
        {
            // Update the text and handle flashing if hitpoints are greater than zero
            if (hitpointsText != null)
            {
                hitpointsText.text = "HP: " + currentHealth.hitpoints;

                if (currentHealth.hitpoints <= 3)
                {
                    if (!isFlashing)
                    {
                        StartCoroutine(FlashHitpointsText());
                    }
                }
                else
                {
                    StopAllCoroutines();
                    hitpointsText.color = originalColor;
                    isFlashing = false;
                }

                // Make sure the text is visible if hitpoints are more than zero
                hitpointsText.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator FlashHitpointsText()
    {
        if (hitpointsText == null) yield break; // Check if hitpointsText is null

        isFlashing = true;

        while (playerHealthGameObject != null && playerHealthGameObject.activeInHierarchy)
        {
            if (hitpointsText == null) yield break; // Check if hitpointsText is null

            hitpointsText.color = hitpointsText.color == originalColor ? Color.red : originalColor;
            yield return new WaitForSeconds(flashInterval);
        }

        // Ensure the text color is reset after exiting the loop
        if (hitpointsText != null)
        {
            hitpointsText.color = originalColor;
        }
        isFlashing = false;
    }
}
