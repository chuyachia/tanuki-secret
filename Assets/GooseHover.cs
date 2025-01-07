using UnityEngine;

public class GooseHover : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.25f; // Maximum distance to move up/down
    [SerializeField] private float frequency = 0.5f; // Speed of the movement
    
    private float randomOffset; // Random offset for the sine wave
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        // Generate a random offset between 0 and 2π
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        // Calculate new Y position using a sine wave with the random offset
        float newY = startPosition.y + amplitude * Mathf.Sin((Time.time * frequency) + randomOffset);
        
        // Update the position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}