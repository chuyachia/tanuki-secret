using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] private CutscenesSO cutsceneToPlay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.InvokeCutsceneEvent(cutsceneToPlay);
        }
    }
}