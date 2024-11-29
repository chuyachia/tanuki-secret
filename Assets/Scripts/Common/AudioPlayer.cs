using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

// Define audio events that correspond to game events
public enum AudioEventType
{
    // SFX
    GetNut,
    putNutInBucket,
    CraneWrongMove,
    CranesSound1,
    CranesSound2,
    CranesSound3,
    CranesSoundCorrect
}

// Audio player that responds to game events
public class AudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioData
    {
        public AudioEventType eventType;
        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;
        public float baseVolume = 1.0f;
        public bool loop = false;
        public float spatialBlend = 0.0f; // 0 = 2D, 1 = 3D
        public float pitch = 1f;
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
        EventManager.Instance.RegisterSquirrelLevelEventListener(HandleSquirrelEvent);
        EventManager.Instance.RegisterCraneLevelEventListener(HandleCraneEvent);

    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.Instance.UnregisterSquirrelLevelEventListener(HandleSquirrelEvent);
        EventManager.Instance.UnregisterCraneLevelEventListener(HandleCraneEvent);
    }

    private void HandleSquirrelEvent(GameObject[] target, EventManager.SquirelLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.SquirelLevelEvent.PickUpNut:
                {
                    OnGetNut(target[0]);
                    break;
                }
            case EventManager.SquirelLevelEvent.PutNutInBucket:
                {
                    OnPutNutInBucket(target[0]);
                    break;
                }
        }
    }

        private void HandleCraneEvent(GameObject[] target, EventManager.CraneLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.CraneLevelEvent.WrongMove:
                {
                    OnWrongMove();
                    break;
                }
            case EventManager.CraneLevelEvent.OtherCranesMove:
                {
                    OnOtherCranesMove();
                    break;
                }
            case EventManager.CraneLevelEvent.CorrectMove:
                {
                    OnCorrectCraneMove();
                    break;
                }

        }
    }

    private void OnCorrectCraneMove()
    {
        PlaySound(AudioEventType.CranesSoundCorrect, transform.position, 1.0f);
    }

    private void OnOtherCranesMove()
    {
        PlaySound(AudioEventType.CranesSound1, transform.position, 1.0f, delay: UnityEngine.Random.Range(0.0f, 0.2f));
        PlaySound(AudioEventType.CranesSound2, transform.position, 0.5f, delay: UnityEngine.Random.Range(0.2f, 0.8f));
        PlaySound(AudioEventType.CranesSound3, transform.position, 0.2f, delay: UnityEngine.Random.Range(0.4f, 0.6f));
    }

    private void OnWrongMove()
    {
        PlaySound(AudioEventType.CraneWrongMove, transform.position, 1.0f);
    }

    // Event handlers
    private void OnPutNutInBucket(GameObject bucket)
    {
        PlaySound(AudioEventType.putNutInBucket, bucket.transform.position, 1.0f);
    }
    private void OnGetNut(GameObject nut)
    {
        PlaySound(AudioEventType.GetNut, nut.transform.position, 1.0f);
    }

    private void PlaySound(AudioEventType eventType, Vector3 position, float volumeMultiplier = 1.0f, float delay = 0.0f)
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
            source.pitch = data.pitch;

            if (data.spatialBlend > 0)
            {
                source.transform.position = position;
            }

            source.PlayDelayed(delay);
        }
    }
}