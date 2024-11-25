using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class achievementScreenManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDoc;

    VisualElement rootEl;
    VisualElement pageContainer;
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
    }

    public void DisplayAchievementScreen(){
        pageContainer.RemoveFromClassList("container--hiden");
        InputControl.menuControlEnabled = true;
        isEndGame = true;
    }

        void Update(){
            if (isEndGame && InputControl.menuControlEnabled && Input.GetKeyDown(KeyCode.E)){
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
    }

}
