using UnityEngine;

public class SquirrelBehaviour : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _airSpeed = 2f;
    [SerializeField] private float _jumpProbability = 0.1f;
    [SerializeField] private float _jumpHeight = 5f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _jumpCooldown = 1f;

    public GameObject Target { set; private get; }

    private float _verticalSpeed = 0f;
    private Vector3 _steerDirection = new Vector3();
    private Vector3 _previousSteerDirection = new Vector3();
    private bool _isGrounded = true;
    private float _groundLevel;
    private float _jumpCooldownTimer = 0f;
    private Vector3 _positionChange = Vector3.zero;

    void Start()
    {
        _groundLevel = transform.position.y;
    }

    Vector3 GetSteerDirection()
    {
        return (Utils.StripYDimension(Target.transform.position) - Utils.StripYDimension(transform.position)).normalized;
    }


    void Steer()
    {
        if (!_isGrounded && _previousSteerDirection != Vector3.zero)
        {
            _steerDirection = _previousSteerDirection;
        }
        else
        {
            _previousSteerDirection = _steerDirection;
            _steerDirection = GetSteerDirection();
            if (_steerDirection == Vector3.zero)
            {
                return;
            }
            // TODO adjust rotation based on real model
            // Quaternion targetRotation = Quaternion.LookRotation(_steerDirection);
            // transform.rotation = targetRotation;
        }
    }


    void CheckJump()
    {
        _jumpCooldownTimer -= Time.deltaTime;
        if (transform.position.y <= _groundLevel)
        {
            _isGrounded = true;
            _verticalSpeed = 0f;
        }
        else
        {
            _isGrounded = false;
        }
    }

    void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _verticalSpeed -= _gravity * Time.deltaTime;
        }
    }

    void Jump()
    {
        if (_isGrounded && _jumpCooldownTimer <= 0f && Random.value < _jumpProbability)
        {
            _verticalSpeed = _jumpHeight;
            _jumpCooldownTimer = _jumpCooldown;
        }
    }

    float GetSpeed()
    {
        return _isGrounded ? _speed : _airSpeed;
    }

    void Update()
    {
        _positionChange = Vector3.zero;
        _steerDirection = Vector3.zero;
        CheckJump();
        ApplyGravity();
        if (Target != null)
        {
            Jump();
            Steer();
        }
        _positionChange += new Vector3(0f, _verticalSpeed, 0f) * Time.deltaTime;
        _positionChange += _steerDirection * GetSpeed() * Time.deltaTime;
        transform.position += _positionChange;
    }
}
