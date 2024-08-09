using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    public GameObject playerHealthGameObject; // Reference to the playerHealth GameObject
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

        UpdateHealthDisplay(); // Initialize health display
    }

    private void Update()
    {
        UpdateHealthDisplay(); // Update health display based on the current tank
    }

    private void UpdateHealthDisplay()
    {
        if (playerHealthGameObject == null || !playerHealthGameObject.activeInHierarchy)
        {
            // Deactivate hitpointsText if playerHealthGameObject is null or inactive
            hitpointsText.gameObject.SetActive(false);
            return; // Exit the method early since there's no need to update the UI
        }

        Health currentHealth = playerHealthGameObject.GetComponent<Health>();

        if (currentHealth == null)
        {
            // Handle the case where the playerHealth component is missing
            hitpointsText.gameObject.SetActive(false);
            return;
        }

        if (currentHealth.hitpoints <= 0)
        {
            // Hide the text if hitpoints are zero or less
            hitpointsText.gameObject.SetActive(false);
        }
        else
        {
            // Update the text and handle flashing if hitpoints are greater than zero
            hitpointsText.text = "HP: " + currentHealth.hitpoints;

            if (currentHealth.hitpoints == 1)
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

    private IEnumerator FlashHitpointsText()
    {
        isFlashing = true;

        while (playerHealthGameObject.activeInHierarchy) // Flash only if playerHealthGameObject is active
        {
            hitpointsText.color = hitpointsText.color == originalColor ? Color.red : originalColor;
            yield return new WaitForSeconds(flashInterval);
        }

        // Ensure the text color is reset after exiting the loop
        hitpointsText.color = originalColor;
        isFlashing = false;
    }
}
