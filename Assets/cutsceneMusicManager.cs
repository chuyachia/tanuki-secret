using System.Collections.Generic;
using UnityEngine;
using System;


public class cutsceneMusicManager : MonoBehaviour
{
    [SerializeField] MusicLayer[] tracksToPlay;
    [SerializeField] MusicLayer[] tracksToStop;

    // Start is called before the first frame update
    void Start()
    {
        StopTracks(tracksToStop);
        PlayTracks(tracksToPlay);
    }

    private void StopTracks(MusicLayer[] tracksToStop)
    {
        if (tracksToStop == null) return;
        foreach (MusicLayer track in tracksToStop)
        {
            EventManager.Instance.InvokeStopMusicLayerEvent(track);
        }
    }
    private void PlayTracks(MusicLayer[] tracksToPlay)
    {
        if (tracksToPlay == null) return;

        foreach (MusicLayer track in tracksToPlay)
        {
            EventManager.Instance.InvokeStartMusicLayerEvent(track);
        }
    }
}
