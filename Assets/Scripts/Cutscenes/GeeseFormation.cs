using UnityEngine;

public class GeeseFormation : MonoBehaviour
{
    public GameObject geesePrefab; // Assign the prefab of your goose model
    public int numGeese = 12; // Number of geese in the flock
    public float formationWidth = 20f; // Width of the formation
    public float formationDepth = 30f; // Depth of the formation
    public float verticalSpacing = 3f; // Vertical spacing between geese
    public float maxVerticalSpacing = 6f; // Maximum vertical spacing

    void Start()
    {
        // Calculate the starting positions for the geese
        Vector3 startPosition = transform.position;
        startPosition.x -= formationWidth / 2;
        startPosition.z -= formationDepth;

        // Instantiate the geese in the formation
        for (int i = 0; i < numGeese; i++)
        {
            // Calculate the position for the current goose
            Vector3 position = startPosition;
            position.x += Mathf.Lerp(-formationWidth / 2, formationWidth / 2, (float)i / (numGeese - 1)); // Horizontal position in V-shape
            position.z += i * (formationDepth / (numGeese - 1)); // Depth position
            position.y += Mathf.Lerp(maxVerticalSpacing, verticalSpacing, (float)i / (numGeese - 1)); // Vertical position with perspective

            // Instantiate the goose at the calculated position
            Instantiate(geesePrefab, position, Quaternion.Euler(0f, 180f, 0f), transform);
        }
    }
}