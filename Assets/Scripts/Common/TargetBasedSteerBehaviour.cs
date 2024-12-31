using UnityEngine;

public abstract class TargetBasedSteerBehaviour : MonoBehaviour
{
    [SerializeField] protected float gravity = -9.81f;
    [SerializeField] protected float jumpHeight = 5f;
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float airSpeed = 2f;

    protected bool isGrounded = true;
    protected float groundLevel;
    protected float verticalSpeed = 0f;
    protected Vector3 steerDirection = new Vector3();
    protected Vector3 targetPosition = Vector3.zero;
    protected Vector3 previousSteerDirection = new Vector3();
    protected float initialXScale;

    protected virtual void Start()
    {
        groundLevel = transform.position.y;
        initialXScale = transform.localScale.x;
    }

    protected virtual void FixedUpdate()
    {
        steerDirection = Vector3.zero;
        UpdateGroundedState();
        ApplyGravity();
        if (ShouldMove())
        {
            Jump();
            Steer();
            FlipModel(steerDirection.x);
        }
        Vector3 positionChange = Vector3.zero;
        positionChange += new Vector3(0f, verticalSpeed, 0f) * Time.fixedDeltaTime;
        positionChange += steerDirection * GetSpeed() * Time.fixedDeltaTime;
        transform.position += positionChange;
    }

    void UpdateGroundedState()
    {
        if (transform.position.y <= groundLevel)
        {
            isGrounded = true;
            verticalSpeed = 0f;
        }
        else
        {
            isGrounded = false;
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalSpeed += gravity * Time.fixedDeltaTime;
        }
    }

    protected virtual bool ShouldJump()
    {
        return false;
    }

    protected virtual bool Jump()
    {
        if (isGrounded && ShouldJump())
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            return true;
        }
        return false;
    }


    protected virtual void Steer()
    {
        if (!isGrounded && previousSteerDirection != Vector3.zero)
        {
            steerDirection = previousSteerDirection;
        }
        else
        {
            previousSteerDirection = steerDirection;
            steerDirection = GetSteerDirection();
        }
    }

    protected virtual bool ShouldMove()
    {
        return targetPosition != transform.position;
    }

    Vector3 GetSteerDirection()
    {
        return (Utils.StripYDimension(targetPosition) - Utils.StripYDimension(transform.position)).normalized;
    }

    void FlipModel(float xMove)
    {
        if (xMove < 0)
        {
            transform.localScale = new Vector3(initialXScale, transform.localScale.y, transform.localScale.z);
        }
        else if (xMove > 0)
        {
            transform.localScale = new Vector3(-initialXScale, transform.localScale.y, transform.localScale.z);
        }
    }

    protected virtual float GetSpeed()
    {
        return isGrounded ? speed : airSpeed;
    }

    public void SetSpeed(float speedValue){
        speed = speedValue;
    }

}
