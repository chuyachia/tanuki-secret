using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeIn = 1f;
    [SerializeField] private float hold = 1f;
    [SerializeField] private float fadeOut = 1f;
    void Start()
    {
        EventManager.Instance.RegisterDeerLevelEventListener(HandleDeerLevelEvent);
        EventManager.Instance.RegisterCraneLevelEventListener(HandleCraneLevelEvent);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterDeerLevelEventListener(HandleDeerLevelEvent);
        EventManager.Instance.UnregisterCraneLevelEventListener(HandleCraneLevelEvent);
    }

    void HandleDeerLevelEvent(GameObject[] target, EventManager.DeerLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.DeerLevelEvent.PlayerTooFarFromDeers:
            case EventManager.DeerLevelEvent.NotEnoughDeersLeft:
                {
                    FadeTransition();
                    return;
                }
        }
    }


    void HandleCraneLevelEvent(GameObject[] target, EventManager.CraneLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.CraneLevelEvent.PlayerTooFarFromCranes:
                {
                    FadeTransition();
                    return;
                }
        }
    }

    void FadeTransition()
    {
        StartCoroutine(FadInThenOut());
    }

    IEnumerator FadInThenOut()
    {
        yield return Fade(1f, fadeIn);
        yield return new WaitForSeconds(hold);
        yield return Fade(0f, fadeOut);
    }


    IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
