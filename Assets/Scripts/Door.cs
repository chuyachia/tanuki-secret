using UnityEngine;

public class Door : MonoBehaviour
{
    private BoxCollider doorCollider;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private GameObject animatedRock;
    
    // Animation parameters
    
    void Start()
    {
        doorCollider = GetComponent<BoxCollider>();
    }

    public void ToggleDoor(bool shouldOpen)
    {
        isOpen = shouldOpen;
        // Change between collision and trigger
        doorCollider.isTrigger = isOpen;
        Destroy(animatedRock, 0.1f);
    }
}
