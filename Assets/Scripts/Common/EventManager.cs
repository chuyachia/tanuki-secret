using UnityEngine;
using UnityEngine.Events;

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

    private UnityEvent<GameObject> getNutEvent;
    private UnityEvent<GameObject> putNutInBucket;
    private UnityEvent<MusicLayer> startMusicLayerEvent;
    private UnityEvent<MusicLayer> stopMusicLayerEvent;

    private EventManager()
    {
        getNutEvent = new UnityEvent<GameObject>();
        putNutInBucket = new UnityEvent<GameObject>();

        // Music events
        startMusicLayerEvent = new UnityEvent<MusicLayer>();
        stopMusicLayerEvent = new UnityEvent<MusicLayer>();
    }

    public void RegisterGetNutEventListener(UnityAction<GameObject> action)
    {
        getNutEvent.AddListener(action);
    }

    public void UnregisterGetNutEventListener(UnityAction<GameObject> action)
    {
        getNutEvent.RemoveListener(action);
    }

    public void RegisterPutNutInBucketEventListener(UnityAction<GameObject> action)
    {
        putNutInBucket.AddListener(action);
    }

    public void UnregisterPutNutInBucketEventListener(UnityAction<GameObject> action)
    {
        putNutInBucket.RemoveListener(action);
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

    public void InvokeGetNutEvent(GameObject nut)
    {
        getNutEvent.Invoke(nut);
    }

    public void InvokePutNutInBucketEvent(GameObject bucket)
    {
        putNutInBucket.Invoke(bucket);
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

}
