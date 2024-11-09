using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AreaChangeTrigger : MonoBehaviour{
    [SerializeField] List<string> textToDisplay;
    private bool firstOccurence = true;

    public event Action<List<string>> AreaEntered;

        private void OnTriggerEnter(Collider other)
        {
            DisplayMessage(); // only displayed the first time the player enters the area. Other logic related to area change could be implemented
        }

    private void DisplayMessage()
    {
        if (firstOccurence)
        {
            AreaEntered?.Invoke(textToDisplay);
            firstOccurence = false;
        }
    }
}


