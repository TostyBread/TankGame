using System.Collections; // Required for IEnumerator
using TMPro; // Required for TextMeshProUGUI
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth; // Reference to the player health script
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
    }

    private void Update()
    {
        if (playerHealth != null)
        {
            if (playerHealth.hitpoints <= 0)
            {
                // Hide the text if hitpoints are zero or less
                hitpointsText.gameObject.SetActive(false);
            }
            else
            {
                // Update the text and handle flashing if hitpoints are greater than zero
                hitpointsText.text = "HP: " + playerHealth.hitpoints;

                if (playerHealth.hitpoints == 1)
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
        isFlashing = true;
        while (playerHealth.hitpoints == 1)
        {
            hitpointsText.color = hitpointsText.color == originalColor ? Color.red : originalColor;
            yield return new WaitForSeconds(flashInterval);
        }
        // Ensure the text color is reset after exiting the loop
        hitpointsText.color = originalColor;
        isFlashing = false;
    }
}

