using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public GameObject defaultTank;
    public GameObject specialTank;
    public GameObject popupText;
    public AudioClip cheatActivatedSFX;
    public float xMinActivationCheat; // Minimum x-axis value before cheat can be activated

    private string cheatCode = "halo";
    private string currentInput = "";
    private float popupDuration = 3f; // Duration for which the text will be displayed

    UIController uiController; // reference UI controller
    public AudioSource audioSource;

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
        uiController = GetComponent<UIController>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Reference AudioSource if not assigned in the Inspector
        }

        SetFollowTarget(defaultTank);
        if (popupText != null)
        {
            popupText.SetActive(false); // Ensure the text is initially hidden
        }
    }

    private void Update()
    {
        DetectInput();
    }

    private void DetectInput()
    {
        if (defaultTank == null || specialTank == null || specialTank.activeInHierarchy) return; // if player has already entered the cheat or tanks already destroyed, ignore it

        float playerX = defaultTank.transform.position.x;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                    {
                        currentInput += keyCode.ToString().ToLower();
                    }
                }
            }

            if (currentInput.EndsWith(cheatCode) && playerX > xMinActivationCheat) //If current input ends with cheat code and player is outside of minimum range
            {
                SwitchTank(false);
                currentInput = "";
                PlaySound(cheatActivatedSFX);
            }
        }

        if (currentInput.Length > cheatCode.Length)
        {
            currentInput = currentInput.Substring(currentInput.Length - cheatCode.Length);
        }
    }

    public void SetFollowTarget(GameObject newTarget)
    {
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
            defaultTank.SetActive(true);
            specialTank.SetActive(false);
            SetFollowTarget(defaultTank);
        }
        else
        {
            specialTank.transform.position = defaultTank.transform.position;
            specialTank.transform.rotation = defaultTank.transform.rotation;

            defaultTank.SetActive(false);
            specialTank.SetActive(true);
            SetFollowTarget(specialTank);
            uiController.tank = GameObject.FindGameObjectWithTag("Player").transform;

            // Show the popup text
            if (popupText != null)
            {
                StartCoroutine(ShowPopupText());
            }
        }
    }

    void PlaySound(AudioClip clip) // Solely handles cheat activation sfx lol
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator ShowPopupText()
    {
        popupText.SetActive(true); // Show the text
        yield return new WaitForSeconds(popupDuration); // Wait for the specified duration
        popupText.SetActive(false); // Hide the text
    }
}
