using UnityEngine;

public class BriefingStuff : MonoBehaviour
{
    public GameObject uncoverBlind;  // The GameObject you want to move
    public float moveSpeed = 2f;  // Speed at which the GameObject will move
    public float targetYPosition = 300f;  // Target Y position

    public GameObject[] gameObjects;  // Array of GameObjects to choose from
    public float[] probabilities;  // Corresponding probabilities for each GameObject

    private float initialYPosition;
    private bool isMoving = false;

    void Start()
    {
        if (gameObjects.Length != probabilities.Length)
        {
            Debug.LogError("GameObjects and probabilities array must be of the same length!");
            return;
        }

        ActivateRandomObject();
        MoveObject();
    }

    void Update()
    {
        if (isMoving)
        {
            Vector3 currentPosition = uncoverBlind.transform.position;
            if (currentPosition.y < targetYPosition)
            {
                currentPosition.y += moveSpeed * Time.deltaTime;
                if (currentPosition.y > targetYPosition)
                {
                    currentPosition.y = targetYPosition;
                    isMoving = false;
                }
                uncoverBlind.transform.position = currentPosition;
            }
        }
    }

    void ActivateRandomObject()
    {
        float totalProbability = 0;
        foreach (float prob in probabilities) totalProbability += prob;

        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        for (int i = 0; i < gameObjects.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                gameObjects[i].SetActive(true);
            }
            else
            {
                gameObjects[i].SetActive(false);
            }
        }
    }

    void MoveObject()
    {
        if (uncoverBlind == null)
        {
            Debug.LogError("Object to move is not assigned!");
            return;
        }

        initialYPosition = uncoverBlind.transform.position.y;
        isMoving = true;
    }
}