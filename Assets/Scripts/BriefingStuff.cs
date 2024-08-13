using UnityEngine;
using System.Collections.Generic;

public class BriefingStuff : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectProbability
    {
        public GameObject gameObject;
        public float probability;
    }

    public GameObject uncoverBlind;  // The GameObject you want to move
    public float moveSpeed = 2f;  // Speed at which the GameObject will move
    public float targetYPosition = 300f;  // Target Y position

    public List<GameObjectProbability> gameObjectProbabilities;  // List of GameObjects with their probabilities

    private float initialYPosition;
    private bool isMoving = false;

    void Start()
    {
        if (gameObjectProbabilities.Count == 0) // if theres nothing in gameobject probability, just skip it
        {
            //Debug.LogError("GameObjectProbabilities list is empty!");
            return;
        }

        ActivateRandomObject(); // activates the commanding officer sprite
        MoveObject(); // moves the blind at soon as it start
    }

    void Update()
    {
        if (isMoving) 
        {
            Vector3 currentPosition = uncoverBlind.transform.position; 
            if (currentPosition.y < targetYPosition) // if haven't reach to target position
            {
                currentPosition.y += moveSpeed * Time.deltaTime; // current position of y axis will be move at certain speed
                if (currentPosition.y > targetYPosition) // if it has reached to target position
                {
                    currentPosition.y = targetYPosition;
                    isMoving = false; // restrict isMoving
                }
                uncoverBlind.transform.position = currentPosition; // blind transform position will be assign to current possion
            }
        }
    }

    void ActivateRandomObject()
    {
        // Deactivate all GameObjects
        foreach (var item in gameObjectProbabilities) // At start, SetActive false all commander sprite
        {
            item.gameObject.SetActive(false);
        }

        // Calculate total probability
        float totalProbability = 0;
        foreach (var item in gameObjectProbabilities)
        {
            totalProbability += item.probability;
        }

        // Pick a random GameObject based on probabilities
        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        foreach (var item in gameObjectProbabilities)
        {
            cumulativeProbability += item.probability;
            if (randomValue <= cumulativeProbability)
            {
                item.gameObject.SetActive(true);
                break;
            }
        }
    }

    void MoveObject()
    {
        if (uncoverBlind == null) // if blind is not available, return;
        {
            //Debug.LogError("Object to move is not assigned!");
            return;
        }

        initialYPosition = uncoverBlind.transform.position.y; // initial position of blind will be assign to uncoverBlind position
        isMoving = true;
    }
}