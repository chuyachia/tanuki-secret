using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] private CutscenesSO cutsceneToPlay;
    [SerializeField] GameTimer gameTimer;
    private bool hasTriggered = false;
    

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            // Disable the trigger collider to prevent any further interactions
            GetComponent<Collider>().enabled = false;
            EventManager.Instance.InvokeCutsceneEvent(cutsceneToPlay);
            gameTimer.StopTimer();
            Debug.Log(gameTimer.GetCurrentTime());
        }
    }
}