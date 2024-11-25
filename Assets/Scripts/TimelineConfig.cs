using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine.Timeline;
using System;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineConfig : MonoBehaviour
{

    private PlayableDirector director;
    private TimelineAsset timeline;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        timeline = getTimeline();
        if (timeline == null) return;
        BindCinemachineTrack(timeline);
        BindSignalReceiver(timeline);
    }

    private TimelineAsset getTimeline()
    {
        // Get the timeline asset
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("No timeline found!");
        }
        return timeline;
    }

    private void BindSignalReceiver(TimelineAsset timeline)
    {
        // Find the CinemachineBrain in the scene
        SignalReceiver signalReceiver = FindObjectOfType<SignalReceiver>();
        if (signalReceiver == null)
        {
            Debug.LogError("No signalReceiver found in the scene!");
            return;
        }

        // Find and bind all CinemachineTrack
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is SignalTrack)
            {
                director.SetGenericBinding(track, signalReceiver);
            }
        }

    }

    private void BindCinemachineTrack(TimelineAsset timeline)
    {
        // Find the CinemachineBrain in the scene
        CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
        if (brain == null)
        {
            Debug.LogError("No CinemachineBrain found in the scene!");
            return;
        }

        // Find and bind all CinemachineTrack
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is CinemachineTrack)
            {
                director.SetGenericBinding(track, brain);
            }
        }
    }
    
}
