using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private CutscenesSO cutsceneSlow;
    [SerializeField] private CutscenesSO cutsceneMedium;
    [SerializeField] private CutscenesSO cutsceneFast;
    [SerializeField] float durationFast = 5.0f;
    [SerializeField] float durationMedium = 10.0f;
    [SerializeField] float durationSlow = 20.0f;
    private float timeRecorded = 0;
    GameTimer gameTimer;

    private void Awake() {
        gameTimer = GetComponent<GameTimer>();
    }

    public void ChooseEndingCutscene()
    {
        gameTimer.StopTimer();
        
        timeRecorded = gameTimer.GetCurrentTime();

        if (timeRecorded <= durationFast){
            EventManager.Instance.InvokeCutsceneEvent(cutsceneFast);
            Debug.Log(timeRecorded);
        }
        else if (timeRecorded <= durationMedium){
            EventManager.Instance.InvokeCutsceneEvent(cutsceneMedium);
            Debug.Log(timeRecorded);
        }
        else if (timeRecorded <= durationSlow){
            EventManager.Instance.InvokeCutsceneEvent(cutsceneSlow);
            Debug.Log(timeRecorded);
        }
        else{
            Debug.Log("Error with timer.");
        }
    }
}
