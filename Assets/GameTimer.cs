using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float currentTime = 0f;
    private bool isTimerRunning = false;

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        currentTime = 0f;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = 0f;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

}
