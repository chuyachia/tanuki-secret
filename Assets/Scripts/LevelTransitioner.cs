using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitioner : MonoBehaviour
{

    [SerializeField] private List<GameObject> tilesToLoad;
    [SerializeField] private List<GameObject> tilesToUnload;

    private Collider transitionCollider;


    private void Start() {
        transitionCollider = GetComponent<Collider>();      
    }

    private void OnTriggerEnter(Collider other) {
        
        foreach (GameObject tileToLoad in tilesToLoad){
            tileToLoad.SetActive(true);
        }

    } 

    private void OnTriggerExit(Collider other) {
        
        foreach (GameObject tileToUnload in tilesToUnload){
            Destroy(tileToUnload, .1f);
        }

        transitionCollider.isTrigger = false;

    }


}
