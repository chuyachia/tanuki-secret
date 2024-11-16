using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitioner : MonoBehaviour
{

    [SerializeField] private List<GameObject> tilesToLoad;
    [SerializeField] private List<GameObject> tilesToUnload;
    [SerializeField] private GameObject backColliderGameObject;
    private Collider transitionCollider;
    



    private void Start() {
        transitionCollider = GetComponent<Collider>();      
    }

    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Player")){
            
            foreach (GameObject tileToLoad in tilesToLoad){
                tileToLoad.SetActive(true);
            }

            foreach (GameObject tileToUnload in tilesToUnload){
                Destroy(tileToUnload, .1f);
            }
            backColliderGameObject.SetActive(true);
        }
    } 

}
