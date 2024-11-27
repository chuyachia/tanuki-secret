using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnimator : MonoBehaviour
{
    Animator anim;
    float randomOffset;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        randomOffset = Random.Range(0f, 0.05f);

        anim.Play("fly", 0, randomOffset);
        
    }
}
