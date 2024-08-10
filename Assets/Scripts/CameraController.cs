using System.Collections;
using UnityEngine;
using Cinemachine;
using TMPro;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public GameObject defaultTank;
    public GameObject specialTank;
    public TextMeshProUGUI popupText; // Add this line

    private string cheatCode = "halo";
    private string currentInput = "";
    private float popupDuration = 3f; // Duration for which the text will be displayed

    UIController uiController; // reference UI controller

    private void Start()
    {
        uiController = GetComponent<UIController>();

        SetFollowTarget(defaultTank);
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false); // Ensure the text is initially hidden
        }
    }

    private void Update()
    {
        DetectInput();
    }

    private void DetectInput()
    {
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

            if (currentInput.EndsWith(cheatCode))
            {
                SwitchTank(false);
                currentInput = "";
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

    private IEnumerator ShowPopupText()
    {
        popupText.gameObject.SetActive(true); // Show the text
        yield return new WaitForSeconds(popupDuration); // Wait for the specified duration
        popupText.gameObject.SetActive(false); // Hide the text
    }
}