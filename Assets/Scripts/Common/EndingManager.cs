using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private CutscenesSO cutsceneSlow;
    [SerializeField] private CutscenesSO cutsceneMedium;
    [SerializeField] private CutscenesSO cutsceneFast;
    [SerializeField] float durationFast = 5.0f;
    [SerializeField] float durationMedium = 10.0f;
    private float timeRecorded = 0;
    GameTimer gameTimer;

    private void Awake()
    {
        gameTimer = GetComponent<GameTimer>();
    }

    public void ChooseEndingCutscene()
    {
        InputControl.charControlEnabled = false;
        gameTimer.StopTimer();
        timeRecorded = gameTimer.GetCurrentTime();

        if (timeRecorded <= durationFast)
        {
            AchievementManager.endingFast = true;
            EventManager.Instance.InvokeCutsceneEvent(cutsceneFast);
        }
        else if (timeRecorded <= durationMedium)
        {
            AchievementManager.endingMedium = true;
            EventManager.Instance.InvokeCutsceneEvent(cutsceneMedium);
        }
        else
        {
            AchievementManager.endingSlow = true;
            EventManager.Instance.InvokeCutsceneEvent(cutsceneSlow);
        }
        Debug.Log("Completed in :" + timeRecorded);
    }
}
