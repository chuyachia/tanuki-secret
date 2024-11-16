using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Define audio events that correspond to game events
public enum AudioEventType
{
    GetNut,
    putNutInBucket,
    // Add more audio events as needed
}

// Audio player that responds to game events
public class AudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioData
    {
        public AudioEventType eventType;
        public AudioClip clip;
        public float baseVolume = 1.0f;
        public bool loop = false;
        public float spatialBlend = 0f; // 0 = 2D, 1 = 3D
    }

    [SerializeField] private AudioData[] audioData;
    private Dictionary<AudioEventType, AudioData> audioMap;
    private Dictionary<AudioEventType, AudioSource> activeSources;

    private void Awake()
    {
        audioMap = new Dictionary<AudioEventType, AudioData>();
        activeSources = new Dictionary<AudioEventType, AudioSource>();

        foreach (var data in audioData)
        {
            audioMap[data.eventType] = data;
        }
    }

    private void OnEnable()
    {
        // Subscribe to events
        EventManager.Instance.RegisterGetNutEventListener(OnGetNut);
       // EventManager.Instance.RegisterPutNutInBucketEventListener(OnPutNutInBucket);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.Instance.UnregisterGetNutEventListener(OnGetNut);
       // EventManager.Instance.RegisterPutNutInBucketEventListener(OnPutNutInBucket);

    }

  //  private void OnPutNutInBucket()
  //  {
//        PlaySound(AudioEventType.putNutInBucket);
  //  }


    // Event handlers
    private void OnGetNut(GameObject nut)
    {
        PlaySound(AudioEventType.GetNut, nut.transform.position);
    }

    private void PlaySound(AudioEventType eventType, Vector3 position, float volumeMultiplier = 1.0f)
    {
        if (audioMap.TryGetValue(eventType, out AudioData data))
        {
            // Reuse or create audio source
            AudioSource source;
            if (!activeSources.TryGetValue(eventType, out source))
            {
                source = gameObject.AddComponent<AudioSource>();
                activeSources[eventType] = source;
            }

            source.clip = data.clip;
            source.volume = data.baseVolume * volumeMultiplier;
            source.loop = data.loop;
            source.spatialBlend = data.spatialBlend;

            if (data.spatialBlend > 0)
            {
                source.transform.position = position;
            }

            source.Play();
        }
    }
}