using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


// observer pattern detecting whether the player changed scene and displaying text accordingly
public class NarrationDisplayer : MonoBehaviour 
{
    [SerializeField] private UIDocument uiDoc; 
    [SerializeField] private List<AreaChangeTrigger> subjectsToObserve;

    private VisualElement rootEl;
    private VisualElement textContainer;
    public static bool messageDisplayCoroutineActive = false; // this ensures that we're not adding text elements if the animation coroutine is being played, the AreaChangeTrigger access it.
    
    private void Awake()
    {
    // subscribe to events (adapt later so it takes a list of events)
    if (subjectsToObserve != null)
        {
            foreach (AreaChangeTrigger subject in subjectsToObserve) {
                subject.AreaEntered += OnThingHappened;
            }
            
        }
    
    // initialize UI
    rootEl = uiDoc.rootVisualElement;
    textContainer = rootEl.Q(className: "narrative-text--container");

    }

    private void OnDestroy() // To prevent error in case the observer is deleted for some reason
    {
        foreach (AreaChangeTrigger subject in subjectsToObserve) {
            subject.AreaEntered -= OnThingHappened;
        }
    }

    private void OnThingHappened(List<string> messages)
    {
        if (!messageDisplayCoroutineActive){
            InitiatilizeSentences(messages);
            StartCoroutine("FadeInSentencesCoroutine");
        }
    }

    private void InitiatilizeSentences(List<string> messages)
    {
        foreach(string message in messages) {
            Label textElLabel = new Label(message);
            textContainer.Add(textElLabel);
            textElLabel.AddToClassList("narative-text--text");
        }
    }

    IEnumerator FadeInSentencesCoroutine()
    {
        messageDisplayCoroutineActive = true;
        
        // Fade in each sentence one by one
        foreach (var child in textContainer.hierarchy.Children()){
            yield return new WaitForSeconds(0.1f);
            child.AddToClassList("narrative-text--text--active");
            yield return new WaitForSeconds(1.5f);
        }
        
        // Wait that the player have read them
        yield return new WaitForSeconds(1.5f);

        // Remove them all
        foreach (var child in textContainer.hierarchy.Children()){
            child.RemoveFromClassList("narrative-text--text--active");
        }
        
        yield return new WaitForSeconds(1f);

        DestroySentenceElements(); // Clean the text container so other trigger sentences can be displayed

        messageDisplayCoroutineActive = false;
    }

    private void DestroySentenceElements()
    {
        var count = textContainer.childCount;

        for (int i=0; i< count; i++){
            textContainer.RemoveAt(0);
        }
    }


}
