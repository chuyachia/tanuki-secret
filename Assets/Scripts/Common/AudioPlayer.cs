using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

// Define audio events that correspond to game events
public enum AudioEventType
{
    // SFX
    GetNut,
    putNutInBucket,
    wrongNut,
    CraneWrongMove,
    CranesSound1,
    CranesSound2,
    CranesSound3,
    CranesSoundCorrect,
    WolfAppear1,
    WolfAppear2,
    WolfAppear3,
    WolfAppear4,
    WolfHit
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
    private List<AudioClip> notes;

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
        EventManager.Instance.RegisterDeerLevelEventListener(HandleDeerEvent);

    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.Instance.UnregisterSquirrelLevelEventListener(HandleSquirrelEvent);
        EventManager.Instance.UnregisterCraneLevelEventListener(HandleCraneEvent);
        EventManager.Instance.UnregisterDeerLevelEventListener(HandleDeerEvent);
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
            case EventManager.SquirelLevelEvent.CorrectBucket:
                {
                    OnPutNutInBucket(target[0], true);
                    break;
                }
            case EventManager.SquirelLevelEvent.WrongBucket:
                {
                    OnPutNutInBucket(target[0], false);
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

    public void HandleDeerEvent(GameObject[] target, EventManager.DeerLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.DeerLevelEvent.WolfAppear:
                {
                    OnWolfAppear(target[0]);
                    break;
                }
            case EventManager.DeerLevelEvent.PlayerAttackWolf:
            {      
                OnPlayerAttackWolf();
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
    private void OnPutNutInBucket(GameObject bucket, bool isCorrectBucket)
    {
        if (isCorrectBucket)
        {
            PlaySound(AudioEventType.putNutInBucket, bucket.transform.position, 1.0f);
        }
        else
        {
            PlaySound(AudioEventType.wrongNut, bucket.transform.position, 1.0f);
        }
    }

    private void OnPlayerAttackWolf()
    {
        PlaySound(AudioEventType.WolfHit, transform.position);
    }

    private void OnWolfAppear(GameObject wolf)
    {


        var noteToPlay = UnityEngine.Random.Range(0, 3);
        if (noteToPlay == 0){
            PlaySound(AudioEventType.WolfAppear1, wolf.transform.position, 1.0f);
            Debug.Log("Played wolf sound 1!");
        }
        if (noteToPlay == 1){
            PlaySound(AudioEventType.WolfAppear2, wolf.transform.position, 1.0f);
            Debug.Log("Played wolf sound 2!");

        }
        if (noteToPlay == 2){
            PlaySound(AudioEventType.WolfAppear3, wolf.transform.position, 1.0f);
            Debug.Log("Played wolf sound 3!");

        }
        if (noteToPlay == 3){
            PlaySound(AudioEventType.WolfAppear4, wolf.transform.position, 1.0f);
            Debug.Log("Played wolf sound 4!");
        }
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