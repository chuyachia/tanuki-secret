using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// Controls the physic movement of a rigid body character.
/// - Arrows keys to move
/// - Space to jump
/// - E to start climb when near climable surface
/// </summary>
public class CharacterControl : MonoBehaviour
{
    public float MoveSpeed = 15f;
    public float ClimbSpeed = 5f;
    public float Acceleration = 10f;
    public float AirAcceleration = 2f;
    public float TurnSpeed = 35f;
    public float JumpForce = 5f;
    public float DecelerateForce = 5f;
    public float MaxGroundAngle = 25f;
    public float ClimbSurfaceRayCastDistance = 10f;
    public LayerMask ClimbableSurface;

    public float AdditionalAccelerationOnSlope = 2f;


    private Rigidbody _rb;

    private bool _onGround;
    private bool _onSlope;
    private bool _hasClimbSurfaceContact;
    private float _minGroundDotProduct;
    private Vector3 _contactNormal = Vector3.up;
    private Vector3 _climbNormal = Vector3.up;
    private float _inputHorizontal;
    private float _inputVertical;
    private bool _shouldClimb;
    private bool _shouldJump;
    private bool _isClimbing;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _minGroundDotProduct = Mathf.Cos(MaxGroundAngle * Mathf.Deg2Rad);
    }

    void Update()
    {
        _inputHorizontal = Input.GetAxis("Horizontal");
        _inputVertical = Input.GetAxis("Vertical");
        _shouldClimb = Input.GetKeyDown(KeyCode.E);
        _shouldJump = Input.GetKeyDown(KeyCode.Space);
        CheckClimbState();
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateGroundCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateGroundCollision(collision);
    }

    void EvaluateGroundCollision(Collision collision)
    {
        int contactNormalCount = 0;
        int climbNormalCount = 0;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(Vector3.up, normal);
            if ((ClimbableSurface.value & (1 << collision.transform.gameObject.layer)) > 0)
            {
                _hasClimbSurfaceContact = true;
                climbNormalCount++;
                _climbNormal += normal;
            }
            else
            {
                if (upDot >= _minGroundDotProduct)
                {
                    _onGround = true;
                }
                else
                {
                    _onSlope = true;
                }
                contactNormalCount++;
                _contactNormal += normal;
            }
        }
        if (climbNormalCount > 1)
        {
            _climbNormal.Normalize();
        }
        if (contactNormalCount > 1)
        {
            _contactNormal.Normalize();
        }
    }

    void Jump()
    {
        if (_onGround && _shouldJump)
        {
            _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 planNormal)
    {
        return vector - planNormal * Vector3.Dot(vector, planNormal);
    }

    float GetAcceleration()
    {
        if (_onSlope && Vector3.Dot(_rb.velocity, Vector3.up) > 0)
        {
            return Acceleration + AdditionalAccelerationOnSlope;

        }
        if (!_onGround)
        {
            return AirAcceleration;
        }
        return Acceleration;
    }

    void Rotate()
    {
        float rotationAmount = 0;
        if (_inputHorizontal != 0)
        {
            rotationAmount = _inputHorizontal * TurnSpeed;
        }
        Quaternion alignToGround = Quaternion.FromToRotation(Vector3.up, _contactNormal);
        Quaternion targetRotation = Quaternion.Euler(alignToGround.eulerAngles.x, _rb.rotation.eulerAngles.y + rotationAmount, alignToGround.eulerAngles.x);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRotation, TurnSpeed * Time.fixedDeltaTime));
    }

    void Move()
    {
        float targetSpeed = _isClimbing ? ClimbSpeed : MoveSpeed;
        if (_rb.velocity.magnitude >= targetSpeed)
        {
            return;
        }
        if (_isClimbing)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y + _inputVertical * ClimbSpeed * Time.fixedDeltaTime, _rb.velocity.z);
        }
        else
        {
            if (_inputVertical != 0)
            {
                Vector3 adjustedForwardDir = ProjectOnContactPlane(transform.forward, _contactNormal).normalized;
                float currentMoveSpeed = Vector3.Dot(_rb.velocity, adjustedForwardDir);
                float desiredMoveSpeed = _inputVertical * MoveSpeed;
                float acceleration = GetAcceleration();
                float maxSpeedChange = acceleration * Time.fixedDeltaTime;
                float newMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, desiredMoveSpeed, maxSpeedChange);
                _rb.velocity += adjustedForwardDir * (newMoveSpeed - currentMoveSpeed);
            }
            else
            {
                _rb.AddForce(-_rb.velocity.normalized * DecelerateForce, ForceMode.Acceleration);
            }
        }
    }

    void ClearState()
    {
        _onSlope = _onGround = _hasClimbSurfaceContact = false;
        _contactNormal = Vector3.up;
    }

    void SnapToSurface()
    {
        if (_onSlope && Vector3.Dot(_rb.velocity, Vector3.down) > 0)
        {
            float speed = _rb.velocity.magnitude;
            float dot = Vector3.Dot(_rb.velocity, _contactNormal);
            _rb.velocity = (_rb.velocity - _contactNormal * dot).normalized * speed;
        }
    }

    void CheckClimbState()
    {
        if (_shouldClimb && _hasClimbSurfaceContact)
        {
            Debug.Log("Start climbing");
            _isClimbing = true;
            return;
        }
        if (!_isClimbing)
        {
            return;
        }

        if (_shouldJump)
        {
            _isClimbing = false;
            Debug.Log("Stop climbing");
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -_climbNormal, out hit, ClimbSurfaceRayCastDistance, ClimbableSurface))
            {
                Debug.Log("Still climbing");
                _isClimbing = true;
            }
            else
            {
                Debug.Log("Fell from climbing");
                _isClimbing = false;
            }
        }
    }

    void FixedUpdate()
    {
        Rotate();
        Move();
        SnapToSurface();
        Jump();
        ClearState();
    }
}
