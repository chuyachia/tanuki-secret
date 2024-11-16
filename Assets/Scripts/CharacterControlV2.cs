using UnityEngine;

public class CharacterControlV2 : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climSpeed = 3f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask climbableSurface;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float climableSurfaceCheckDistance = 2f;

    private Vector3 velocity;

    private float inputHorizontal;
    private float inputVertical;
    private bool shouldInteract;
    private bool shouldJump;
    private bool isClimbing;
    private Vector3 climbDirection;
    private float initialSlopeLimit;
    private bool hasNut;
    private bool collisionHit;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        initialSlopeLimit = characterController.slopeLimit;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (collisionHit)
        {
            return;
        }
        if (hit.gameObject.CompareTag("Nut") && hit.gameObject.activeSelf && !hasNut)
        {
            Debug.Log("Pick up a nut");
            EventManager.Instance.InvokeGetNutEvent(hit.gameObject);
            hasNut = true;
            collisionHit = true;

            return;
        }
        if (hit.gameObject.CompareTag("NutBucket") && hasNut)
        {
            Debug.Log("Put nut in bucket");
            EventManager.Instance.InvokePutNutInBucketEvent();
            hasNut = false;
            collisionHit = true;
            return;
        }
    }

    void CheckClimbState()
    {

        RaycastHit hitInfo;
        if (shouldInteract && Physics.Raycast(transform.position, transform.forward, out hitInfo, climableSurfaceCheckDistance, climbableSurface))
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
        collisionHit = false;
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        shouldInteract = Input.GetKeyDown(KeyCode.E);
        shouldJump = Input.GetKeyDown(KeyCode.Space);
    }

    void FixedUpdate()
    {
        CheckClimbState();
        if (isClimbing)
        {
            Climb();
        }
        else
        {
            Move();
        }
    }

    void Climb()
    {
        characterController.Move(climbDirection * climSpeed * inputVertical * Time.deltaTime);
    }

    void Move()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        if (isGrounded)
        {
            characterController.slopeLimit = initialSlopeLimit;
        }

        Vector3 moveDirection = transform.right * inputHorizontal + transform.forward * inputVertical;

        if (isGrounded)
        {
            if (shouldJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                velocity.y = -2f;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        Vector3 move = moveDirection.normalized * moveSpeed;
        characterController.Move((move + velocity) * Time.deltaTime);
    }
}