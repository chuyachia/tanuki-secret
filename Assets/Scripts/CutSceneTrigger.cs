using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] private EndingManager endingManager;
    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            // Disable the trigger collider to prevent any further interactions
            GetComponent<Collider>().enabled = false;
            endingManager.ChooseEndingCutscene();
        }
    }

}