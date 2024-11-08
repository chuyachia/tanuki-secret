using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public Vector3 Offset;
    public float SmoothSpeed = 0.5f;

    void Start()
    {
        transform.position = Player.position + Offset;
    }

    void LateUpdate()
    {
        if (Player != null)
        {
            Vector3 desiredPosition = Player.position + Offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
