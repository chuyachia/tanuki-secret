using UnityEngine;

public class CraneBehaviour : MonoBehaviour
{
    private CraneDanceAnimator craneDanceAnimator;
    private Vector3 targetPosition;
    private float moveTimer;
    private float moveDuration;

    void Start()
    {
        craneDanceAnimator = new CraneDanceAnimator(transform.GetChild(0).gameObject);
    }

    public void Dance(DanceMove danceMove)
    {
        craneDanceAnimator.Dance(danceMove);
    }

    public void MoveTo(Vector3 position, float duration)
    {
        targetPosition = position;
        moveDuration = duration;
        moveTimer = 0f;
    }

    void Update()
    {
        if (moveTimer < moveDuration)
        {
            float t = Mathf.Clamp01(moveTimer / moveDuration);
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            moveTimer += Time.deltaTime;
        }
    }
}
