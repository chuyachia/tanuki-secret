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
    private UnityEvent putNutInBucket;

    private EventManager()
    {
        getNutEvent = new UnityEvent<GameObject>();
        putNutInBucket = new UnityEvent();
    }

    public void RegisterGetNutEventListener(UnityAction<GameObject> action)
    {
        getNutEvent.AddListener(action);
    }

    public void UnregisterGetNutEventListener(UnityAction<GameObject> action)
    {
        getNutEvent.RemoveListener(action);
    }

    public void RegisterPutNutInBucketEventListener(UnityAction action)
    {
        putNutInBucket.AddListener(action);
    }

    public void UnregisterPutNutInBucketEventListener(UnityAction action)
    {
        putNutInBucket.RemoveListener(action);
    }


    public void InvokeGetNutEvent(GameObject nut)
    {
        getNutEvent.Invoke(nut);
    }

    public void InvokePutNutInBucketEvent()
    {
        putNutInBucket.Invoke();
    }
}