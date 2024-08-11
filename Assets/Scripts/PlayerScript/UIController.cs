using UnityEngine;

public class UIController : MonoBehaviour
{
    public RectTransform tankUI; // Assign the parent RectTransform for health and reload UI
    public Transform tank; // Reference to the tank's transform
    public float offset = 50f; // Small offset from the tank
    public float smoothSpeed = 5f; // Speed at which the UI moves to the target position

    private void Start()
    {
        // Find the tank GameObject with the tag "Player"
        tank = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (tank == null) return;

        PositionUI();
    }

    private void PositionUI()
    {
        // Convert the tank's world position to screen position
        Vector2 tankScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, tank.position);

        // Get the mouse position in world space
        Vector2 mouseScreenPosition = Input.mousePosition;

        // Calculate the direction from the tank to the mouse position
        Vector2 directionToMouse = (mouseScreenPosition - tankScreenPosition).normalized;

        // Calculate the new UI position by moving in the opposite direction of the mouse
        Vector2 targetPosition = tankScreenPosition - directionToMouse * offset;

        // Smoothly move the UI's position toward the target position
        tankUI.position = Vector2.Lerp(tankUI.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}