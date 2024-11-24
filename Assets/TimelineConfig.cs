using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineConfig : MonoBehaviour
{

    private PlayableDirector director;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        BindCinemachineTrack();
    }

    private void BindCinemachineTrack()
    {
        // Get the timeline asset
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null) return;

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
