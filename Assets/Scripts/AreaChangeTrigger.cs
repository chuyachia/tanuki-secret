using System.Collections.Generic;
using UnityEngine;
using System;

public class AreaChangeTrigger : MonoBehaviour
{
    [SerializeField] Level currentLevel;
    [SerializeField] List<string> textToDisplay;
    [SerializeField] MusicLayer[] tracksToPlay;
    [SerializeField] MusicLayer[] tracksToStop;

    private bool firstOccurence = true;

    public event Action<List<string>> AreaEntered;

    private void OnTriggerEnter(Collider other)
    {
        OnAreaEnter(); // only displayed the first time the player enters the area. Other logic related to area change could be implemented
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

    private void OnAreaEnter()
    {

        if (firstOccurence & !NarrationDisplayer.messageDisplayCoroutineActive)
        {
            AreaEntered?.Invoke(textToDisplay);
            EventManager.Instance.InvokeLevelEnterEvent(currentLevel);
            firstOccurence = false;
        }
    }
}


