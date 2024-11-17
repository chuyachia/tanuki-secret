using UnityEngine;

public class CharacterControlV2 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float airSpeed = 1f;
    [SerializeField] private float climSpeed = 3f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask climbableSurface;
    [SerializeField] private float climableSurfaceCheckDistance = 2f;


    private CharacterController characterController;
    private Animator animationController;
    private Vector3 velocity;
    private float inputHorizontal;
    private float inputVertical;
    private bool shouldInteract;
    private bool shouldJump;
    private bool isClimbing;
    private bool isGrounded;
    private Vector3 climbDirection;
    private float initialSlopeLimit;
    private bool hasNut;
    private bool collisionHit;
    private float initialXScale;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animationController = GetComponentInChildren<Animator>();
        initialSlopeLimit = characterController.slopeLimit;
        initialXScale = transform.localScale.x;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (collisionHit)
        {
            return;
        }
        if (hit.gameObject.CompareTag("Nut") && hit.gameObject.activeSelf && !hasNut)
        {
            EventManager.Instance.InvokeGetNutEvent(hit.gameObject);
            hasNut = true;
            collisionHit = true;

            return;
        }
        if (hit.gameObject.CompareTag("NutBucket") && hasNut)
        {
            EventManager.Instance.InvokePutNutInBucketEvent(hit.gameObject);
            hasNut = false;
            collisionHit = true;
            return;
        }
    }

    void UpdateClimbState()
    {

        if (shouldInteract && Physics.Raycast(transform.position, transform.forward, climableSurfaceCheckDistance, climbableSurface))
        {
            isClimbing = true;
            characterController.slopeLimit = 90f;
            return;
        }
        if (!isClimbing)
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, climableSurfaceCheckDistance, climbableSurface))
        {
            climbDirection = Vector3.Cross(hit.normal, -transform.right).normalized;
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
            climbDirection = Vector3.zero;
        }
    }

    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        shouldInteract = Input.GetKeyDown(KeyCode.E);
        shouldJump = Input.GetKeyDown(KeyCode.Space);
    }

    void FixedUpdate()
    {
        ResetCollistionState();
        UpdateClimbState();
        if (isClimbing)
        {
            Climb();
        }
        else
        {
            UpdateGroundedState();
            ApplyGravity();
            FlipModelOnHorizontalInput();
            Jump();
            Move();
        }
    }

    void ResetCollistionState()
    {
        collisionHit = false;
    }

    void Climb()
    {
        characterController.Move(climSpeed * inputVertical * Time.fixedDeltaTime * climbDirection);
    }

    void UpdateGroundedState()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded)
        {
            characterController.slopeLimit = initialSlopeLimit;
        }
        animationController.SetBool("isGrounded", isGrounded);
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
    }

    float GetSpeed()
    {
        return isGrounded ? moveSpeed : airSpeed;
    }

    void Jump()
    {
        if (isGrounded && shouldJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Move()
    {
        Vector3 moveDirection = transform.right * inputHorizontal + transform.forward * inputVertical;
        Vector3 move = moveDirection.normalized * GetSpeed();
        if (move != Vector3.zero)
        {
            animationController.SetBool("isWalking", true);
        }
        else
        {
            animationController.SetBool("isWalking", false);
        }

        characterController.Move((move + velocity) * Time.fixedDeltaTime);
    }

    void FlipModelOnHorizontalInput()
    {
        if (inputHorizontal < 0)
        {
            transform.localScale = new Vector3(initialXScale, transform.localScale.y, transform.localScale.z);
        }
        else if (inputHorizontal > 0)
        {
            transform.localScale = new Vector3(-initialXScale, transform.localScale.y, transform.localScale.z);
        }
    }
}