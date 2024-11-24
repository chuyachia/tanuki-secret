using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Event manager singleton responsible for registering event listener and invoking event
/// </summary>
public class EventManager
{
    // singleton
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventManager();
            }
            return _instance;
        }
    }

    public enum SquirelLevelEvent
    {
        PickUpNut,
        PutNutInBucket
    }

    public enum DeerLevelEvent
    {
        WolfCatchDeer,
        WolfFlee,

        DeerArrivedAtDestination,
        PlayerTooFarFromDeers,
        NotEnoughDeersLeft
    }

    private UnityEvent<GameObject[], SquirelLevelEvent> squirrelLevelEvent;
    private UnityEvent<GameObject[], DeerLevelEvent> deerLevelEvent;
    private UnityEvent<MusicLayer> startMusicLayerEvent;
    private UnityEvent<MusicLayer> stopMusicLayerEvent;
    private UnityEvent<Level> levelEnterEvent;
    private UnityEvent<CutscenesSO> cutsceneEvent;
    private UnityEvent<List<string>> cutsceneMessageEvent;

    private EventManager()
    {
        // Level events
        squirrelLevelEvent = new UnityEvent<GameObject[], SquirelLevelEvent>();
        deerLevelEvent = new UnityEvent<GameObject[], DeerLevelEvent>();
        
        // Music events
        startMusicLayerEvent = new UnityEvent<MusicLayer>();
        stopMusicLayerEvent = new UnityEvent<MusicLayer>();

        // Game flow events
        levelEnterEvent = new UnityEvent<Level>();
        cutsceneEvent = new UnityEvent<CutscenesSO>();
        cutsceneMessageEvent = new UnityEvent<List<string>>();
    }

    public void RegisterSquirrelLevelEventListener(UnityAction<GameObject[], SquirelLevelEvent> action)
    {
        squirrelLevelEvent.AddListener(action);
    }

    public void UnregisterSquirrelLevelEventListener(UnityAction<GameObject[], SquirelLevelEvent> action)
    {
        squirrelLevelEvent.RemoveListener(action);
    }

    public void RegisterDeerLevelEventListener(UnityAction<GameObject[], DeerLevelEvent> action)
    {
        deerLevelEvent.AddListener(action);
    }

    public void UnregisterDeerLevelEvenListener(UnityAction<GameObject[], DeerLevelEvent> action)
    {
        deerLevelEvent.RemoveListener(action);
    }

    // Register methods
    public void RegisterStartMusicLayerEventListener(UnityAction<MusicLayer> listener)
    {
        startMusicLayerEvent.AddListener(listener);
    }

    public void RegisterStopMusicLayerEventListener(UnityAction<MusicLayer> listener)
    {
        stopMusicLayerEvent.AddListener(listener);
    }

    // Unregister methods
    public void UnregisterStartMusicLayerEventListener(UnityAction<MusicLayer> listener)
    {
        startMusicLayerEvent.RemoveListener(listener);
    }

    public void UnregisterStopMusicLayerEventListener(UnityAction<MusicLayer> listener)
    {
        stopMusicLayerEvent.RemoveListener(listener);
    }

    public void RegisterLevelEnterEventListener(UnityAction<Level> listner)
    {
        levelEnterEvent.AddListener(listner);
    }

    public void UnregisterLevelEnterEventListener(UnityAction<Level> listner)
    {
        levelEnterEvent.RemoveListener(listner);
    }

    public void RegisterCutsceneEventListener(UnityAction<CutscenesSO> listener)
    {
        cutsceneEvent.AddListener(listener);
    }

    public void UnregisterCutsceneEventListener(UnityAction<CutscenesSO> listener)
    {
        cutsceneEvent.RemoveListener(listener);
    }

    public void RegisterCutsceneMessageEventListener(UnityAction<List<string>> listener)
    {
        cutsceneMessageEvent.AddListener(listener);
    }

    public void UnregisterCutsceneMessageEventListener(UnityAction<List<string>> listener)
    {
        cutsceneMessageEvent.RemoveListener(listener);
    }

    public void InvokeSquirrelLevelEvent(GameObject[] target, SquirelLevelEvent eventType)
    {
        squirrelLevelEvent.Invoke(target, eventType);
    }

    public void InvokeDeerLevelEvent(GameObject[] target, DeerLevelEvent eventType)
    {
        deerLevelEvent.Invoke(target, eventType);
    }

    // Invoke methods
    public void InvokeStartMusicLayerEvent(MusicLayer layer)
    {
        startMusicLayerEvent.Invoke(layer);
    }

    public void InvokeStopMusicLayerEvent(MusicLayer layer)
    {
        stopMusicLayerEvent.Invoke(layer);
    }


    public void InvokeLevelEnterEvent(Level level)
    {
        levelEnterEvent.Invoke(level);
    }

    public void InvokeCutsceneEvent(CutscenesSO cutscene)
    {
        cutsceneEvent.Invoke(cutscene);
    }

    public void InvokeCutsceneMessageEvent(List<string> messages)
    {
        cutsceneMessageEvent.Invoke(messages);
    }

}
