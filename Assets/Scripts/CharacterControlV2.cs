using UnityEngine;

public class CharacterControlV2 : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climSpeed = 3f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private LayerMask climbableSurface;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float climableSurfaceCheckDistance = 2f;

    private Vector3 velocity;

    private float inputHorizontal;
    private float inputVertical;
    private bool shouldClimb;
    private bool shouldJump;
    private bool isClimbing;
    private Vector3 climbDirection;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Nut") && hit.gameObject.activeSelf)
        {

            EventManager.Instance.InvokeGetNutEvent(hit.gameObject);
            return;
        }
        if (hit.gameObject.CompareTag("NutBucket"))
        {

            Debug.Log("Put nut in bucket");
            return;
        }
    }

    void CheckClimbState()
    {

        RaycastHit hitInfo;
        if (shouldClimb && Physics.Raycast(transform.position, transform.forward, out hitInfo, climableSurfaceCheckDistance, climbableSurface))
        {
            isClimbing = true;
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
        shouldClimb = Input.GetKeyDown(KeyCode.E);
        shouldJump = Input.GetKeyDown(KeyCode.Space);
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

        if (isGrounded && Vector3.Angle(Vector3.up, characterController.velocity) > maxSlopeAngle)
        {
            velocity = new Vector3(0, velocity.y, 0);
        }

        Vector3 move = moveDirection * moveSpeed;
        characterController.Move((move + velocity) * Time.deltaTime);
    }
}