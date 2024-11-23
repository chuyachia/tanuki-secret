using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "CutsceneSO")]
public class CutscenesSO : ScriptableObject
{
    [SerializeField] private bool pauseControls;
    [SerializeField] private bool pauseGame;
    [SerializeField] private bool pauseTimer;
    [SerializeField] PlayableDirector cutScene;
    [SerializeField] List<string> messagesToDisplay;
    
    [SerializeField] float messagesDelay = 2f; 

}
