using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayNote : MonoBehaviour
{
    
    [SerializeField] private Texture[] correctNotesTextures; // not yet implemented
    [SerializeField] private Texture[] wrongNoteTextures; // not yet implemented
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color correctColor = Color.blue;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private float colorMultiplier = 5.0f;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float correctRiseSpeed = 1f;
    [SerializeField] private float incorrectJumpForce = 5f;
    [SerializeField] private float incorrectRotationSpeed = 45f;
    
    private GameObject player;
    private Material material;
    private Coroutine hideCoroutine;
    private bool isCorrectMove = false;
    private bool isMoving = false;
    private float verticalVelocity = 0f;
    private const float gravity = -9.81f;

    private void Awake()
    {
        material = meshRenderer.material;
        gameObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ShowNote(bool isCorrect)
    {
        transform.parent = player.transform;
        transform.localPosition = transform.localPosition = new Vector3(Random.Range(-3f, -1.5f), Random.Range(4f, 5f), 0);
        isCorrectMove = isCorrect;

        // Random flip
        transform.localScale = new Vector3(
            1,
            Random.Range(0, 2) * 2 - 1,
            1
        );
        
        // Sample random texture
        Texture selectedTexture = isCorrect 
            ? correctNotesTextures[Random.Range(0, correctNotesTextures.Length)] 
            : wrongNoteTextures[Random.Range(0, wrongNoteTextures.Length)];
        material.SetTexture("_Texture", selectedTexture);

        material.SetColor("_basecolor", isCorrect ? correctColor * colorMultiplier: incorrectColor * colorMultiplier);


        isMoving = true;
        verticalVelocity = isCorrect ? 0 : incorrectJumpForce;
        
        if (!isCorrect)
        {
            transform.parent = null; // Detach for falling animation
        }

        // Show and play effects
        gameObject.SetActive(true);
        if (hideCoroutine != null){
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    void Update()
    {
        if (!isMoving) return;
        
        if (isCorrectMove)
        {
            transform.localPosition += Vector3.up * correctRiseSpeed * Time.deltaTime;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
            transform.Rotate(0, 0, incorrectRotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        isMoving = false;
        gameObject.SetActive(false);
        transform.rotation = new Quaternion(0,0,0,0); // reinitialize rotation
    }
}
