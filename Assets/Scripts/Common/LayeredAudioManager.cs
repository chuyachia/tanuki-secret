using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Audio;
// Define our audio layers

public enum MusicLayer
{
    Drums,
    Strings,
    Piano,
    Synths,
    
    // Ambient sounds
    Crickets

}

public class LayeredAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class MusicLayerData
    {
        public MusicLayer layer;
        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;
        public float volume = 0.7f;
        public float fadeTime = 1f;
        public bool playOnStart = false;
    }

    [Header("Audio Setup")]
    [SerializeField] private MusicLayerData[] musicLayers;
    
    private Dictionary<MusicLayer, AudioSource> musicSources = new Dictionary<MusicLayer, AudioSource>();
    private Dictionary<AudioSource, Coroutine> fadeCoroutines = new Dictionary<AudioSource, Coroutine>();

    public static LayeredAudioManager Instance { get; private set; }

    private void Awake()
    
    {
        if (Instance != null && Instance != this){ 
            Destroy(this); 
        } 
        else{ 
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        } 

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
            source.outputAudioMixerGroup = layer.audioMixerGroup;
            
            if (layer.playOnStart)
            {
                StartMusicLayer(layer.layer);
            }
        }
    }

    private void SubscribeToEvents()
    {
        EventManager.Instance.RegisterStartMusicLayerEventListener(StartMusicLayer);
        EventManager.Instance.RegisterStopMusicLayerEventListener(StopMusicLayer);
        EventManager.Instance.RegisterStopAllMusicLayersEventListener(StopAllMusicLayers);
    }

    private void OnDestroy()
    {
        // Make sure to unregister when the object is destroyed
        EventManager.Instance.UnregisterStartMusicLayerEventListener(StartMusicLayer);
        EventManager.Instance.UnregisterStopMusicLayerEventListener(StopMusicLayer);
        EventManager.Instance.UnregisterStopAllMusicLayersEventListener(StopAllMusicLayers);
    }


    // Example event handler
    private void OnStartMusicLayer(MusicLayer musicLayer)
    {
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

    public void StopAllMusicLayers(){
        
        foreach(var musicLayer in musicLayers){
            StopMusicLayer(musicLayer.layer);
        }

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