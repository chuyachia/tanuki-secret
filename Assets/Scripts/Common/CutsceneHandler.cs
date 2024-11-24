using UnityEngine;
using UnityEngine.Playables;

public class CutsceneHandler : MonoBehaviour
{
    // Reference to the CutscenePlayer
    [SerializeField] private CutscenePlayer cutscenePlayer;
    
    // Add your cutscene ScriptableObjects here
    [SerializeField] private CutscenesSO[] cutscenes;
    
    private void Start()
    {
        // Register for events that should trigger cutscenes
        RegisterEventListeners();
    }
    
    private void OnDestroy()
    {
        // Clean up event listeners
        UnregisterEventListeners();
    }
    
    private void RegisterEventListeners()
    {
        // Example: Register for squirrel level events
        EventManager.Instance.RegisterCutsceneEventListener(OnCutsceneEvent);
    }
    
    private void UnregisterEventListeners()
    {
        // Example: Unregister from squirrel level events
        EventManager.Instance.UnregisterCutsceneEventListener(OnCutsceneEvent);
    }
    
    private void OnCutsceneEvent(CutscenesSO cutscene)
    {
            cutscenePlayer.PlayCutscene(cutscene);
    }
    
}
