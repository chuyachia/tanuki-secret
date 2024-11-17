using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
// Define our audio layers

public enum MusicLayer
{
    Drums,
    Strings,
    Guitar,
    Leads
}

public enum AmbientType
{
    Crickets
    // Add more ambient types as needed
}

public class LayeredAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class MusicLayerData
    {
        public MusicLayer layer;
        public AudioClip clip;
        public float volume = 0.7f;
        [Range(0, 1)] public float currentVolume = 0f;
        public float fadeTime = 1f;
    }

    [System.Serializable]
    public class AmbientSoundData
    {
        public AmbientType type;
        public AudioClip clip;
        public float volume = 0.5f;
        [Range(0, 1)] public float currentVolume = 0f;
        public float fadeTime = 2f;
        public bool playOnStart = false;
    }

    [Header("Audio Setup")]
    [SerializeField] private MusicLayerData[] musicLayers;
    [SerializeField] private AmbientSoundData[] ambientSounds;
    
    private Dictionary<MusicLayer, AudioSource> musicSources = new Dictionary<MusicLayer, AudioSource>();
    private Dictionary<AmbientType, AudioSource> ambientSources = new Dictionary<AmbientType, AudioSource>();
    private Dictionary<AudioSource, Coroutine> fadeCoroutines = new Dictionary<AudioSource, Coroutine>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeAudioSources();
        SubscribeToEvents();
    }

    private void InitializeAudioSources()
    {
        // Setup music layers
        foreach (var layer in musicLayers)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.clip = layer.clip;
            source.volume = 0;
            source.loop = true;
            source.playOnAwake = false;
            musicSources[layer.layer] = source;
        }

        // Setup ambient sounds
        foreach (var ambient in ambientSounds)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.clip = ambient.clip;
            source.volume = 0;
            source.loop = true;
            source.playOnAwake = false;
            ambientSources[ambient.type] = source;

            if (ambient.playOnStart)
            {
                StartAmbientSound(ambient.type);
            }
        }
    }

    private void SubscribeToEvents()
    {
        EventManager.Instance.RegisterStartMusicLayerEventListener(StartMusicLayer);
        EventManager.Instance.RegisterStopMusicLayerEventListener(StopMusicLayer);
    }

    private void OnDestroy()
    {
        // Make sure to unregister when the object is destroyed
        EventManager.Instance.UnregisterStartMusicLayerEventListener(StartMusicLayer);
        EventManager.Instance.UnregisterStopMusicLayerEventListener(StopMusicLayer);
    }


    // Example event handler
    private void OnStartMusicLayer(MusicLayer musicLayer)
    {
        // Example: Start synths after first transition
        StartMusicLayer(musicLayer);
    }

    public void StartMusicLayer(MusicLayer layer)
    {
        if (!musicSources.TryGetValue(layer, out AudioSource source)) return;

        MusicLayerData layerData = musicLayers.FirstOrDefault(l => l.layer == layer);
        if (layerData == null) return;

        if (!source.isPlaying)
        {
            source.Play();
        }

        StartFade(source, layerData.volume, layerData.fadeTime);
    }

    public void StopMusicLayer(MusicLayer layer)
    {
        if (!musicSources.TryGetValue(layer, out AudioSource source)) return;

        MusicLayerData layerData = musicLayers.FirstOrDefault(l => l.layer == layer);
        if (layerData == null) return;

        StartFade(source, 0f, layerData.fadeTime, true);
    }

    public void StartAmbientSound(AmbientType type)
    {
        if (!ambientSources.TryGetValue(type, out AudioSource source)) return;

        AmbientSoundData ambientData = ambientSounds.FirstOrDefault(a => a.type == type);
        if (ambientData == null) return;

        if (!source.isPlaying)
        {
            source.Play();
        }

        StartFade(source, ambientData.volume, ambientData.fadeTime);
    }

    public void StopAmbientSound(AmbientType type)
    {
        if (!ambientSources.TryGetValue(type, out AudioSource source)) return;

        AmbientSoundData ambientData = ambientSounds.FirstOrDefault(a => a.type == type);
        if (ambientData == null) return;

        StartFade(source, 0f, ambientData.fadeTime, true);
    }

    private void StartFade(AudioSource source, float targetVolume, float fadeTime, bool stopAfterFade = false)
    {
        // Stop any existing fade for this source
        if (fadeCoroutines.TryGetValue(source, out Coroutine existingCoroutine))
        {
            StopCoroutine(existingCoroutine);
        }

        // Start new fade
        fadeCoroutines[source] = StartCoroutine(FadeRoutine(source, targetVolume, fadeTime, stopAfterFade));
    }

    private IEnumerator FadeRoutine(AudioSource source, float targetVolume, float fadeTime, bool stopAfterFade)
    {
        float startVolume = source.volume;
        float elapsed = 0;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeTime);
            yield return null;
        }

        source.volume = targetVolume;
        
        if (stopAfterFade && Mathf.Approximately(targetVolume, 0f))
        {
            source.Stop();
        }

        fadeCoroutines.Remove(source);
    }
}