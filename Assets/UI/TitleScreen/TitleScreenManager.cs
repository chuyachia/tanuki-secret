using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDoc;
    [SerializeField] private PlayableDirector timelineDirector;
    CharacterControlV2 characterControls;
    private VisualElement rootEl;
    private VisualElement titleTextContainer;
    private VisualElement pressStartTextContainer; // the element where we indicate that the player has to press a button to start
    
    

    private void Awake()
    {
        GetUIElements();
        characterControls = FindObjectOfType<CharacterControlV2>();
    }

    private void GetUIElements()
    {
        rootEl = uiDoc.rootVisualElement;
        titleTextContainer = rootEl.Q(className: "main-title");
        pressStartTextContainer = rootEl.Q(className: "press-start-text");
    }

    // display Title - disable all controls
    private void Start() {
        rootEl = uiDoc.rootVisualElement;
        titleTextContainer = rootEl.Q(className: "main-title");
        pressStartTextContainer = rootEl.Q(className: "press-start-text");

        characterControls = FindObjectOfType<CharacterControlV2>();
        DisableCharControls();
        DisableMenuControls();
        StartCoroutine("TitleScreenAppearSequence");
    }

    IEnumerator TitleScreenAppearSequence(){
        yield return new WaitForSeconds(0.5f);
        titleTextContainer.AddToClassList("element--active");
        yield return new WaitForSeconds(2.5f);
        pressStartTextContainer.AddToClassList("element--active");
        yield return new WaitForSeconds(1.0f);
        InputControl.menuControlEnabled = true;
    }

    void Update(){
        if (InputControl.menuControlEnabled && Input.GetKeyDown(KeyCode.E)){
            StartCoroutine("StartGameSequence");
        }
    }

    IEnumerator StartGameSequence(){
        InputControl.menuControlEnabled = false;
        yield return new WaitForSeconds(0.5f);
        titleTextContainer.RemoveFromClassList("element--active");
        pressStartTextContainer.RemoveFromClassList("element--active");
        yield return new WaitForSeconds(1.0f);
        timelineDirector.Play();
        yield return new WaitForSeconds(1.0f);
        InputControl.charControlEnabled = true;
        gameObject.SetActive(false);
    }



    private void DisableCharControls() {
        InputControl.charControlEnabled = false;
    }

    private void DisableMenuControls()
    {
        InputControl.menuControlEnabled = false;
    }


    private void EnableControls() {
        characterControls.enabled = true;
    }


    // display press any button and allow player actions

    // Make title screen disappear

    // Play animation bringing the player to the game

    // restore controls when cinematics ended

}
