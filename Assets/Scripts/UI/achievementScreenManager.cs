using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class achievementScreenManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDoc;
    VisualElement rootEl;
    VisualElement pageContainer;
    // Slow ending
    VisualElement achivementSlowIcon;
    VisualElement achivementSlowText;
    // Medium ending
    VisualElement achivementMediumIcon;
    VisualElement achivementMediumText;
    // Fast ending
    VisualElement achivementFastIcon;
    VisualElement achivementFastText;
    // Credits
    VisualElement creditsContainer;
    // Press E to restart
    VisualElement bottomContainer;
    VisualElement transitionOverlay;

    private string currentSceneName;
    private bool isEndGame = false;
    
    private void Awake()
    {
        GetUIElements();
    }

    private void GetUIElements()
    {
        rootEl = uiDoc.rootVisualElement;
        pageContainer = rootEl.Q(className: "container");
        // Slow ending
        achivementSlowIcon = rootEl.Q(className: "achievement--slow--icon");
        achivementSlowText = rootEl.Q(className: "achievement--slow--text-container");
        // Medium ending
        achivementMediumIcon = rootEl.Q(className: "achievement--medium--icon");
        achivementMediumText = rootEl.Q(className: "achievement--medium--text-container");
        // Fast ending
        achivementFastIcon = rootEl.Q(className: "achievement--fast--icon");
        achivementFastText = rootEl.Q(className: "achievement--fast--text-container");
        // Credits
        creditsContainer = rootEl.Q(className: "credits");
        // Press E to restart
        bottomContainer = rootEl.Q(className: "bottom-text");
        // Fade to black transition
        transitionOverlay = rootEl.Q(className: "transition--overlay");
    }

    public void DisplayAchievementScreen(){
        SelectAchivementsToDisplay();
        pageContainer.RemoveFromClassList("container--hiden");
        isEndGame = true;
        StartCoroutine("StartAchivementMenuSequence");
    }

    private void SelectAchivementsToDisplay()
    {
        if (AchievementManager.endingSlow){
            achivementSlowIcon.AddToClassList("achivement--slow--icon--achieved");
            achivementSlowText.RemoveFromClassList("achievement--slow--text-container--hidden");
        }
        if (AchievementManager.endingMedium){
            achivementMediumIcon.AddToClassList("achievement--medium--icon--achieved");
            achivementMediumText.RemoveFromClassList("achievement--medium--text-container-hidden");
        }
        if (AchievementManager.endingFast){
            achivementFastIcon.AddToClassList("achivement--fast--achieved");
            achivementFastText.RemoveFromClassList("achievement--fast--text-container--hiden");
        }
        if (AchievementManager.endingSlow && AchievementManager.endingMedium && AchievementManager.endingFast){
            creditsContainer.RemoveFromClassList("credits--hidden");
        }
    }

    void Update(){
            if (isEndGame && InputControl.menuControlEnabled && Input.GetKeyDown(KeyCode.E)){
                StartCoroutine("RestartGame");
            }
    }


    private  IEnumerator StartAchivementMenuSequence(){
        yield return  new WaitForSeconds(2.0f);
        bottomContainer.RemoveFromClassList("bottom-text--hidden");
        InputControl.menuControlEnabled = true;
    }

    private IEnumerator RestartGame(){
                string currentSceneName = SceneManager.GetActiveScene().name;
                EventManager.Instance.InvokeStopAllMusicLayersEvent();
                transitionOverlay.RemoveFromClassList("transition--overlay--hidden");
                yield return new WaitForSeconds(1.0f);
                SceneManager.LoadScene(currentSceneName);

    }
}
