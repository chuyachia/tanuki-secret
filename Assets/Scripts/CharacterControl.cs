using System;
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
    public float MaxGroundAngle = 25f;
    public float MinClimbaleAngle = 60f;
    public float ClimbSurfaceRayCastDistance = 10f;
    public LayerMask ClimbableSurface;
    public float ModelHeight = 2.5f;

    public float AdditionalAccelerationOnSlope = 2f;
    public MoveType moveType;
    public RotateType rotateType;


    private Rigidbody _rb;

    private bool _onGround;
    private bool _onSlope;
    private bool _hasClimbSurfaceContact;
    private float _minGroundDotProduct;
    private float _maxClimbDotProduct;
    private Vector3 _contactNormal = Vector3.zero;
    private Vector3 _climbDir = Vector3.zero;
    private float _inputHorizontal;
    private float _inputVertical;
    private bool _shouldClimb;
    private bool _shouldJump;
    private bool _isClimbing;

    public enum MoveType
    {
        FromPlayerPerspective,
        InWordSpace
    }

    public enum RotateType
    {
        Continuous,
        Discrete
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _minGroundDotProduct = Mathf.Cos(MaxGroundAngle * Mathf.Deg2Rad);
        _maxClimbDotProduct = Mathf.Cos(MinClimbaleAngle * Mathf.Deg2Rad);
    }

    void Update()
    {
        _inputHorizontal = Input.GetAxis("Horizontal");
        _inputVertical = Input.GetAxis("Vertical");
        _shouldClimb = Input.GetKeyDown(KeyCode.E);
        _shouldJump = Input.GetKeyDown(KeyCode.Space);
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
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(Vector3.up, normal);
            if (upDot < _maxClimbDotProduct && (ClimbableSurface.value & (1 << collision.transform.gameObject.layer)) > 0)
            {
                _hasClimbSurfaceContact = true;
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

    void ContinuousRotate()
    {
        float rotationAmount = 0;
        if (_inputHorizontal != 0)
        {
            rotationAmount = _inputHorizontal * TurnSpeed;
        }
        // Align to ground
        // Quaternion alignToGround = Quaternion.FromToRotation(transform.up, _contactNormal);
        // Quaternion targetRotation = Quaternion.Euler(alignToGround.eulerAngles.x, _rb.rotation.eulerAngles.y + rotationAmount, alignToGround.eulerAngles.x);

        Quaternion targetRotation = Quaternion.Euler(_rb.rotation.eulerAngles.x, _rb.rotation.eulerAngles.y + rotationAmount, _rb.rotation.eulerAngles.z);
        _rb.rotation = targetRotation;
    }

    void MoveInWordSpace()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right, _contactNormal).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward, _contactNormal).normalized;

        float currentX = Vector3.Dot(_rb.velocity, xAxis);
        float currentZ = Vector3.Dot(_rb.velocity, zAxis);

        float acceleration = GetAcceleration();
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;
        Vector3 inputMovement = new Vector3(_inputHorizontal, 0f, _inputVertical);
        if (inputMovement == Vector3.zero)
        {
            return;
        }

        Vector3 targetVelocity = inputMovement * MoveSpeed;
        float newX = Mathf.MoveTowards(currentX, targetVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, targetVelocity.z, maxSpeedChange);
        Vector3 currentVelocity = xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        _rb.velocity += currentVelocity;
    }

    void DiscreteRotate()
    {
        if (_inputVertical > 0)
        {
            _rb.rotation = Quaternion.LookRotation(Vector3.forward);
        }
        else if (_inputVertical < 0)
        {
            _rb.rotation = Quaternion.LookRotation(Vector3.back);
        }
        if (_inputHorizontal > 0)
        {
            _rb.rotation = Quaternion.LookRotation(Vector3.right);
        }
        else if (_inputHorizontal < 0)
        {
            _rb.rotation = Quaternion.LookRotation(Vector3.left);
        }
    }

    void MoveFromPlayerPerspective()
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
    }


    void Climb()
    {
        _rb.velocity += _climbDir * ClimbSpeed * _inputVertical * Time.fixedDeltaTime;
    }

    void ClearState()
    {
        _onSlope = _onGround = _hasClimbSurfaceContact = false;
        _contactNormal = Vector3.zero;
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
        if (_shouldClimb &&
        _hasClimbSurfaceContact &&
         Physics.Raycast(transform.position, transform.forward, ClimbSurfaceRayCastDistance, ClimbableSurface))
        {
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
            _climbDir = Vector3.zero;
            Debug.Log("Stop climbing");
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, ClimbSurfaceRayCastDistance, ClimbableSurface))
            {
                Debug.DrawRay(transform.position, transform.forward, Color.red, 5f);
                Debug.DrawRay(hit.point, hit.normal, Color.blue, 5f);
                _climbDir = Vector3.Cross(hit.normal, -transform.right).normalized; // only works when facing climb object
                Debug.DrawRay(transform.position, _climbDir, Color.black, 5f);

                Debug.Log("Still climbing");
                _isClimbing = true;
            }
            else
            {
                Debug.Log("Fell from climbing");
                _isClimbing = false;
                _climbDir = Vector3.zero;
            }
        }
    }

    bool WithinSpeedLimit()
    {
        float targetSpeed = _isClimbing ? ClimbSpeed : MoveSpeed;
        if (_rb.velocity.magnitude >= targetSpeed)
        {
            return false;
        }
        return true;
    }

    void FixedUpdate()
    {
        CheckClimbState();
        if (WithinSpeedLimit())
        {
            if (_isClimbing)
            {
                Climb();
            }
            else
            {
                switch (moveType)
                {
                    case MoveType.FromPlayerPerspective:
                        MoveFromPlayerPerspective();
                        break;
                    case MoveType.InWordSpace:
                        MoveInWordSpace();
                        break;
                }
            }
        }
        switch (rotateType)
        {
            case RotateType.Continuous:
                ContinuousRotate();
                break;
            case RotateType.Discrete:
                DiscreteRotate();
                break;
        }
        SnapToSurface();
        Jump();
        ClearState();
    }
}
