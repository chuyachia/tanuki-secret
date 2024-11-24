using UnityEngine;
using UnityEngine.Playables;

public class CutscenePlayer : MonoBehaviour
{
    private PlayableDirector activeDirector;
    private CutscenesSO currentCutscene;
    private GameObject cutsceneInstance;
    
    public void PlayCutscene(CutscenesSO cutsceneData)
    {
        if (cutsceneData == null || cutsceneData.ScenePrefab == null)
        {
            Debug.LogError("Invalid cutscene data provided!");
            return;
        }

        StopCurrentCutscene();
        currentCutscene = cutsceneData;
            
        // Instantiate the cutscene prefab
        cutsceneInstance = Instantiate(currentCutscene.ScenePrefab);
        
        // Get and play the PlayableDirector
        activeDirector = cutsceneInstance.GetComponent<PlayableDirector>();
        if (activeDirector != null)
        {
            activeDirector.stopped += OnCutsceneComplete;
            activeDirector.Play();
        }
        
        // If there are messages to display, invoke the event
        if (currentCutscene.MessagesToDisplay != null && 
            !NarrationDisplayer.messageDisplayCoroutineActive)
        {
            // We'll add a new event type for cutscene messages
            //EventManager.Instance.InvokeCutsceneMessageEvent(currentCutscene.MessagesToDisplay);
        }
    }

    private void OnCutsceneComplete(PlayableDirector director)
    {
        StopCurrentCutscene();
    }

    private void StopCurrentCutscene()
    {
        if (activeDirector != null)
        {
            activeDirector.stopped -= OnCutsceneComplete;
            activeDirector.Stop();
        }
            
        if (cutsceneInstance != null){
            Destroy(cutsceneInstance);
        }
                
        currentCutscene = null;
        activeDirector = null;
    }
}