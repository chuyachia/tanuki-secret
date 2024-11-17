using TMPro;
using UnityEngine;

public class SquirrelBehaviour : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _airSpeed = 2f;
    [SerializeField] private float _jumpProbability = 0.1f;
    [SerializeField] private float _jumpHeight = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpCooldown = 1f;
    [SerializeField] private float _pauseBeforeGoToTarget = 1f;

    public GameObject Target
    {
        set
        {
            _target = value;
            if (value != null && value.CompareTag(Constants.Tags.NutBucket))
            {
                _steerTowardsTargetTimer = _pauseBeforeGoToTarget;
            }
        }
    }
    private float _steerTowardsTargetTimer;
    private GameObject _target;
    private float _verticalSpeed = 0f;
    private Vector3 _steerDirection = new Vector3();
    private Vector3 _previousSteerDirection = new Vector3();
    private bool _isGrounded = true;
    private float _groundLevel;
    private float _jumpCooldownTimer = 0f;
    private float _initialXScale;
    private Animator _animator;

    void Start()
    {
        _groundLevel = transform.position.y;
        _initialXScale = transform.localScale.x;
        _animator = GetComponentInChildren<Animator>();
    }

    Vector3 GetSteerDirection()
    {
        return (Utils.StripYDimension(_target.transform.position) - Utils.StripYDimension(transform.position)).normalized;
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
        }
    }

    void UpdateGroundedState()
    {
        if (_jumpCooldownTimer > 0)
        {
            _jumpCooldownTimer -= Time.fixedDeltaTime;
        }
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
            _verticalSpeed += _gravity * Time.fixedDeltaTime;
        }
    }

    void Jump()
    {
        if (_isGrounded && _jumpCooldownTimer <= 0f && Random.value < _jumpProbability)
        {
            _verticalSpeed = Mathf.Sqrt(_jumpHeight * -2f * _gravity); ;
            _jumpCooldownTimer = _jumpCooldown;
        }
    }

    float GetSpeed()
    {
        return _isGrounded ? _speed : _airSpeed;
    }

    void FlipModel(float xMove)
    {
        if (xMove < 0)
        {
            transform.localScale = new Vector3(_initialXScale, transform.localScale.y, transform.localScale.z);
        }
        else if (xMove > 0)
        {
            transform.localScale = new Vector3(-_initialXScale, transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        _steerDirection = Vector3.zero;
        UpdateGroundedState();
        if (_steerTowardsTargetTimer > 0)
        {
            _steerTowardsTargetTimer -= Time.fixedDeltaTime;
        }
        _animator.SetBool(Constants.AnimatorState.IsGrounded, _isGrounded);
        ApplyGravity();
        if (_target != null && _steerTowardsTargetTimer <= 0)
        {
            Jump();
            Steer();
            _animator.SetBool(Constants.AnimatorState.IsWalking, true);
        }
        else
        {
            _animator.SetBool(Constants.AnimatorState.IsWalking, false);
        }
        FlipModel(_steerDirection.x);
        Vector3 positionChange = Vector3.zero;
        positionChange += new Vector3(0f, _verticalSpeed, 0f) * Time.fixedDeltaTime;
        positionChange += _steerDirection * GetSpeed() * Time.fixedDeltaTime;
        transform.position += positionChange;
    }
}
