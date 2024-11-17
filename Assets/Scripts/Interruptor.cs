using UnityEngine;

public class Interruptor : MonoBehaviour
{
    [SerializeField] private Door connectedDoor; // Array of doors this interruptor controls
    private SphereCollider triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<SphereCollider>();
        // Make sure it's set as trigger
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player entering the trigger
        if (other.CompareTag(Constants.Tags.Player))
        {
            connectedDoor.ToggleDoor(true);
        }
    }

}