using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine virtual camera
    public GameObject defaultTank; // Reference to the default tank GameObject
    public GameObject specialTank; // Reference to the special tank GameObject
    public GameObject popupText; // Reference to the popup text GameObject that displays cheat activation
    public AudioClip cheatActivatedSFX; // AudioClip played when the cheat is activated
    public float xMinActivationCheat; // Minimum x-axis value before the cheat can be activated

    private string cheatCode = "halo"; // The cheat code to be activated
    private string currentInput = ""; // Current input string to check against the cheat code
    private float popupDuration = 3f; // Duration for which the popup text will be displayed

    UIController uiController; // Reference to the UIController component
    public AudioSource audioSource; // Reference to the AudioSource for playing sound effects

    // Property to check if the camera is currently following the special tank
    public bool IsFollowingSpecialTank
    {
        get
        {
            if (virtualCamera == null || virtualCamera.Follow == null)
            {
                return false;
            }
            return virtualCamera.Follow.gameObject == specialTank;
        }
    }

    private void Start()
    {
        uiController = GetComponent<UIController>(); // Get the UIController component attached to the same GameObject

        // Reference AudioSource if not assigned in the Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Set the camera to follow the default tank initially
        SetFollowTarget(defaultTank);

        // Ensure the popup text is initially hidden
        if (popupText != null)
        {
            popupText.SetActive(false);
        }
    }

    private void Update()
    {
        DetectInput(); // Continuously check for user input
    }

    private void DetectInput()
    {
        // Check if either tank is not assigned or the special tank is active
        if (defaultTank == null || specialTank == null || specialTank.activeInHierarchy) return;

        float playerX = defaultTank.transform.position.x; // Get the x position of the default tank

        // Check if any key is pressed
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                // If a letter key is pressed, append its lowercase representation to currentInput
                if (Input.GetKeyDown(keyCode) && keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                {
                    currentInput += keyCode.ToString().ToLower();
                }
            }

            // Check if the current input ends with the cheat code and player is within activation range
            if (currentInput.EndsWith(cheatCode) && playerX > xMinActivationCheat)
            {
                SwitchTank(false); // Switch to the special tank
                currentInput = ""; // Reset the current input string
                PlaySound(cheatActivatedSFX); // Play the cheat activation sound
            }
        }

        // Ensure currentInput only retains the length of the cheat code
        if (currentInput.Length > cheatCode.Length)
        {
            currentInput = currentInput.Substring(currentInput.Length - cheatCode.Length);
        }
    }

    public void SetFollowTarget(GameObject newTarget)
    {
        // Set the camera's follow and look-at targets to the new GameObject
        if (virtualCamera != null)
        {
            virtualCamera.Follow = newTarget.transform;
            virtualCamera.LookAt = newTarget.transform;
        }
    }

    public void SwitchTank(bool useDefaultTank)
    {
        if (useDefaultTank)
        {
            // Activate default tank and deactivate special tank
            defaultTank.SetActive(true);
            specialTank.SetActive(false);
            SetFollowTarget(defaultTank); // Set camera to follow the default tank
        }
        else
        {
            // Move special tank to the default tank's position and rotation
            specialTank.transform.position = defaultTank.transform.position;
            specialTank.transform.rotation = defaultTank.transform.rotation;

            // Activate special tank and deactivate default tank
            defaultTank.SetActive(false);
            specialTank.SetActive(true);
            SetFollowTarget(specialTank); // Set camera to follow the special tank
            uiController.tank = GameObject.FindGameObjectWithTag("Player").transform; // Update UIController with the new player tank

            // Show the popup text indicating cheat activation
            if (popupText != null)
            {
                StartCoroutine(ShowPopupText());
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        // Play the provided sound clip if the AudioSource and clip are valid
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator ShowPopupText()
    {
        // Display the popup text
        popupText.SetActive(true);
        yield return new WaitForSeconds(popupDuration); // Wait for the specified duration
        // Hide the popup text
        popupText.SetActive(false);
    }
}
