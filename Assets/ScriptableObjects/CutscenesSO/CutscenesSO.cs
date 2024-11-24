using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "CutsceneSO")]
public class CutscenesSO : ScriptableObject
{
    [SerializeField] private bool pauseControls;
    [SerializeField] private GameObject scenePrefab;
    [SerializeField] private List<string> messagesToDisplay;
    [SerializeField] private float messagesDelay = 2f;

    // Public properties
    public bool PauseControls => pauseControls;
    public GameObject ScenePrefab => scenePrefab;
    public List<string> MessagesToDisplay => messagesToDisplay;
    public float MessagesDelay => messagesDelay;
}