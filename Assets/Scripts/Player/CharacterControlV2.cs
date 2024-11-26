using System.Collections;
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
    [SerializeField] private Level defaultLevel;


    private CharacterController characterController;
    private ModelController modelController;
    private PlayerBaseBehaviour playerLevelBehaviour;
    private Vector3 velocity;
    private Vector3 move;
    private float inputHorizontal;
    private float inputVertical;
    private bool inputInteract;
    private bool inputSpace;
    private bool isClimbing;
    private bool isGrounded;
    private Vector3 climbDirection;
    private float initialSlopeLimit;
    private bool collisionHit;
    private float initialXScale;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        modelController = GetComponent<ModelController>();
        initialSlopeLimit = characterController.slopeLimit;
        initialXScale = transform.localScale.x;
        playerLevelBehaviour = new PlayerBaseBehaviour(modelController);
        EventManager.Instance.RegisterLevelEnterEventListener(ChangeBehaviour);
        ChangeBehaviour(defaultLevel);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterLevelEnterEventListener(ChangeBehaviour);

    }

    void ChangeBehaviour(Level level)
    {
        playerLevelBehaviour.Cleanup();
        switch (level)
        {
            case Level.Squirrel:
                {
                    playerLevelBehaviour = new PlayerSquirrelBehaviour(transform, modelController);
                    break;
                }
            case Level.Deer:
                {
                    playerLevelBehaviour = new PlayerDeerBehaviour(transform, modelController);
                    break;
                }
            case Level.Crane:
                {
                    playerLevelBehaviour = new PlayerCraneBehaviour(transform, modelController);
                    break;
                }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (collisionHit)
        {
            return;
        }
        playerLevelBehaviour.HandleControllerColliderHit(hit);
    }

    void UpdateClimbState()
    {

        if (inputInteract && Physics.Raycast(transform.position, transform.forward, climableSurfaceCheckDistance, climbableSurface))
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
        if (InputControl.charControlEnabled)
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");
            inputInteract = Input.GetKeyDown(KeyCode.E);
            inputSpace = Input.GetKey(KeyCode.Space);
        }
        if (playerLevelBehaviour is PlayerCraneBehaviour && !playerLevelBehaviour.ShouldMove())
        {
            PlayerCraneBehaviour craneBehaviour = (PlayerCraneBehaviour)playerLevelBehaviour;
            craneBehaviour.CraneDanse(inputHorizontal, inputVertical, inputSpace);
            if (inputHorizontal > 0)
            {
                characterController.SimpleMove(Vector3.right * 4f);
            }
            else if (inputHorizontal < 0)
            {
                characterController.SimpleMove(Vector3.left * 4f);
            }
        }
    }

    void FixedUpdate()
    {
        ResetCollistionState();
        UpdateClimbState();
        if (isClimbing)
        {
            Climb();
        }
        else if (playerLevelBehaviour.ShouldMove())
        {
            UpdateGroundedState();
            ApplyGravity();
            FlipModelOnHorizontalInput();
            Jump();
            Move();
            playerLevelBehaviour.UpdateAnimatorBasedOnMovement(move, isGrounded);
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
        if (isGrounded && inputSpace)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Move()
    {
        Vector3 moveDirection = transform.right * inputHorizontal + transform.forward * inputVertical;
        move = moveDirection.normalized * GetSpeed();
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
